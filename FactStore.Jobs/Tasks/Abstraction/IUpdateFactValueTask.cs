using FactStore.Models;
using System.Threading.Tasks;

namespace FactStore.Jobs.Tasks
{
    public interface IUpdateFactValueTask
    {
        Task Invoke(ExternalFactConfig externalFactConfig);
    }
}