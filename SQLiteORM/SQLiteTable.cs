using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteORM
{
    class SQLiteTable
    {
        private string _name;
        private SQLiteRow _headRow;
        private SortedList<long, List<string>> _bodyRows;
        public SQLiteRow HeadRowInfo
        {
            get
            {
                return _headRow;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
        }
        public SortedList<long, List<string>> BodyRows
        {
            get
            {
                return _bodyRows;
            }
        }
        public SQLiteTable(string tableName, SQLiteRow headRow, SortedList<long, List<string>> bodyRows)
        {
            _name = tableName; //валидация?
            _headRow = headRow;
            _bodyRows = bodyRows;
        }

        public void AddOneRow(List<string> row)
        {
            this.BodyRows.Add(_bodyRows.Count + 1, row);

            string query = $"INSERT INTO {_name} (";
            foreach (SQLiteColumn column in _headRow)
            {
                query += column.Name + ", ";
            }
            query = query.Substring(0, query.Length - 2);
            query += $") VALUES ( {_bodyRows.Count + 1}, ";

            foreach (string item in row)
            {
                query += "'" + item + "', ";
            }
            query = query.Substring(0, query.Length - 2);
            query += ")";

            SQLiteDBEngine.AsyncQuery.Add(query);
        }
        public void DeleteOneRow(long Id)
        {
            if (BodyRows.ContainsKey(Id))
            {
                BodyRows.Remove(Id);
            }
            else
            {
                throw new ArgumentException("Incorrect row Id");
            }
        }
        public KeyValuePair<long, List<string>> GetOneRow(long Id)
        {
            if (BodyRows.ContainsKey(Id))
            {
                return new KeyValuePair<long, List<string>>(Id, BodyRows[Id]);
            }
            else
            {
                throw new ArgumentException("Incorrect row Id");
            }
        }
        public KeyValuePair<long, List<string>>? GetOneRow(List<KeyValuePair<string, string>> searchPattern)
        {
            bool searchMatched;
            foreach (KeyValuePair<long, List<string>> oneRow in BodyRows)
            {
                searchMatched = true;
                foreach (KeyValuePair<string, string> pattern in searchPattern)
                {
                    int indexCol = HeadRowInfo[pattern.Key].Cid;

                    if (indexCol != 0)
                    {
                        if (oneRow.Value[indexCol - 1] != pattern.Value)
                        {
                            searchMatched = false;
                            break;
                        }
                    }
                    else
                    {
                        if (oneRow.Key != Int64.Parse(pattern.Value))
                        {
                            searchMatched = false;
                            break;
                        }
                    }
                }
                if (searchMatched)
                {
                    return oneRow;
                }
            }
            return null;
        }
        public bool UpdateOneRow(long Id, List<string> newData)
        {
            throw new NotImplementedException();
        }
    }
}
