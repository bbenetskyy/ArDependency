﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulasCollection.Realizations
{
    public class ResultForForks
    {
        public string Event;
        public string Type;
        public string Coef;
        public string Bookmaker { get; set; }
        //  X1 X2 1 2 
        public ResultForForks() { }
        public ResultForForks(string nameTeam1, string nameTeam2, string date, string nameCoff, string coef, string type, string bookmaker)
        {
            this.Event = nameTeam1.Trim() + " - " + nameTeam2.Trim();
            this.MatchDateTime = date;
            this.Type = nameCoff;
            this.Coef = coef;
            SportType = type;
            Bookmaker = bookmaker;
        }

        public string SportType { get; set; }
        public string MatchDateTime { get; set; }
    }
}