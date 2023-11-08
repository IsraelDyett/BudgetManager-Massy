using System.Data;

namespace BudgetManager.Repository
{
    public interface ICustomer
    {
        string DocumentUpload(IFormFile formFile);
        DataTable CustomerTable(string path);

        void ImportCustomer(DataTable customer);

    }
}
