using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ris.Data;
using Ris.Data.Models;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using System.ComponentModel;
using Ris.Client.Models;

namespace Ris.Client.WinRT
{
    //
    // http://www.silverlightplayground.org/post/2012/06/10/Metro-Incrementally-load-GridView-and-ListView-with-ISupportIncrementalLoading.aspx
    // http://stackoverflow.com/questions/10527358/isupportincrementalloading-only-fires-once
    // http://msdn.microsoft.com/en-us/library/windows/apps/Hh701916 (XAML data binding sample)
    // http://blogs.msdn.com/b/devosaure/archive/2012/10/15/isupportincrementalloading-loading-a-subsets-of-data.aspx
    // http://michelsalib.com/2012/10/21/winrt-how-to-properly-implement-isupportincrementalloading-with-navigation/
    //
    public class RisQueryWithIncrementalLoading : ObservableCollection<DocumentReference>, ISupportIncrementalLoading
    {
        public static async Task<SearchResult> LoadPage(RisQueryParameter queryParam, int seitenNummer)
        {
            var risClient = new RisClient();
            var result = await risClient.QueryAsync(queryParam, seitenNummer);
            
            return result;
        }

        public RisQueryWithIncrementalLoading()
        {
        }

        public RisQueryWithIncrementalLoading(RisQueryParameter queryParam, SearchResult result, 
            Action incrementalLoadStarted, Action incrementalLoadCompleted, Action<string> incrementalLoadFailed)
        {
            QueryParameter = queryParam;

            Hits = result.Hits;
            Page = result.Page;
            PageSize = result.PageSize;

            foreach (var dr in result.DocumentReferences) Add(dr);

            _incrementalLoadStarted = incrementalLoadStarted;
            _incrementalLoadCompleted = incrementalLoadCompleted;
            _incrementalLoadFailed = incrementalLoadFailed;
        }

        public RisQueryParameter QueryParameter { get; set; }

        public int? Hits { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        private readonly Action _incrementalLoadStarted;
        private readonly Action _incrementalLoadCompleted;
        private readonly Action<string> _incrementalLoadFailed;

        public bool HasMoreItems
        {
            get
            {
                if (!Hits.HasValue || !Page.HasValue || !PageSize.HasValue) return false;
                return (Hits > Page * PageSize);
            }
        }

        public Windows.Foundation.IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            CoreDispatcher dispatcher = Window.Current.Dispatcher;

            if (null != _incrementalLoadStarted) _incrementalLoadStarted();

            return Task.Run<LoadMoreItemsResult>(
                async () =>
                {
                    var result = await LoadPage(QueryParameter, Page.Value);

                    if (!result.Succeeded)
                    {
                        dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal,
                            () =>
                            {
                                if (null != _incrementalLoadFailed) _incrementalLoadFailed(result.Error);
                            });

                        return new LoadMoreItemsResult() { Count = 0 };
                    }

                    dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        () =>
                        {
                            foreach (var dr in result.DocumentReferences) Add(dr);

                            if (null != _incrementalLoadCompleted) _incrementalLoadCompleted();
                        });

                    return new LoadMoreItemsResult() { Count = (uint)result.DocumentReferences.Count };
                }).AsAsyncOperation<LoadMoreItemsResult>();
        }
    }
}
