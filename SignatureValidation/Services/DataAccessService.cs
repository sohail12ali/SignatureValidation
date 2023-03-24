using SignatureValidation.Constants;
using SignatureValidation.Helpers;
using SignatureValidation.Models;

using System.IO;

namespace SignatureValidation.Services
{
    public class DataAccessService
    {
        private readonly FileHelper fileHelper;
        public List<HashRepoModel> HashRepoModels { get; private set; } = new List<HashRepoModel>();

        public DataAccessService(FileHelper fileHelper)
        {
            this.fileHelper = fileHelper;
            _ = LoadRepo();
        }

        /// <summary>
        /// Load Repo File to list
        /// </summary>
        /// <param name="repoPath"></param>
        /// <returns>void</returns>
        public async Task LoadRepo(string? repoPath = null)
        {
            try
            {
                HashRepoModels.Clear();
                string path = repoPath ?? Path.Combine(AppConstants.DataFolderName, AppConstants.HashRepoFileName);
                var list = await fileHelper.LoadFullCsv<HashRepoModel>(path);
                Guard.IsNotNull(list);
                Guard.IsTrue(list.Any());
                HashRepoModels.AddRange(list);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Get Repo Data from the list
        /// </summary>
        /// <returns>List of HashRepoModel</returns>
        public async ValueTask<IEnumerable<HashRepoModel>> GetRepo(string? repoPath = null)
        {
            if (HashRepoModels != null && HashRepoModels.Any())
            {
                return HashRepoModels;
            }
            await LoadRepo(repoPath);
            return HashRepoModels!;
        }
    }
}