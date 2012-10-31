﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Ris.Data.Models
{
    public class DbRisQueryParameter
    {
        public DbRisQueryParameter()
        {
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Executed { get; set; }
        public int? Hits { get; set; }

        // RisFulltextQueryParameter
        public string FulltextSearchString { get; set; }

        public DbRisQueryParameter(string searchText, int? hits)
        {
            FulltextSearchString = searchText;
            Hits = hits;

            Executed = DateTime.Now;
        }

        private RisQueryParameter _transformed = null;

        [Ignore]
        public RisQueryParameter RisQueryParameter
        {
            get
            {
                if (null != _transformed) return _transformed;

                if (!String.IsNullOrWhiteSpace(FulltextSearchString))
                {
                    _transformed = new RisFulltextQueryParameter(FulltextSearchString);
                }

                return _transformed;
            }
        }


        // Advanced Search

    }
}