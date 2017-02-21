using System.Windows.Threading;

namespace TTools.Domain
{
    public class DrivedObject : DispatcherObject
    {
        public void DoSomething()
        {
            this.VerifyAccess();
        }
    }
}
