using PostApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostApp.Repositories
{
    public interface IMailManRepository
    {
        /// <summary>
        /// Brevbäraren ger dig alla brev hen samlat in sedan du bad om de senast.
        /// </summary>
        /// <returns>Alla nya brev</returns>
        /// <exception cref="OperationCanceledException">Kastas när det inte finns någon brevbärare vid postkontoret</exception>
        List<Package> GetMail();

        /// <summary>
        /// Brevbäraren tar alla brev och beger sig ut för att leverera de
        /// </summary>
        /// <param name="letters">breven du vill skicka</param>
        /// <exception cref="DirectoryNotFoundException">Kastas när brevbäraren inte hittar mottagaren</exception>
        void SendMails(List<Package> letters);

    }
}
