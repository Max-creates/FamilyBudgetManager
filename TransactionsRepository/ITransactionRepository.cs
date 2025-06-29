﻿using System.Data;

namespace FamilyBudgetManager.TransactionsRepository
{
    public interface ITransactionRepository
    {
        DataTable ReadAllTransactions(string tableName);
        void Write(string category, string description, string amount, DateTime date);
        void Update(int id, string category, string description, string amount, DateTime date);
        void Delete(int id);
        void CreateNewIfNotExists();
        double GetSumFromCategory(string typeOfTransaction);
        public List<string> GetAllTableNames();
    }
}
