using LangBot.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangBot.Web.Services
{
    public class DialogService
    {
        private readonly DatabaseRepo _databaseRepo;

        public DialogService(DatabaseRepo databaseRepo)
        {
            _databaseRepo = databaseRepo;
        }

        public async Task CreateDialog(string actionTimeStamp, object dialog)
        {
            throw new NotImplementedException();
        }
    }
}
