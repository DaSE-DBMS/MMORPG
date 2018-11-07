using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvTipInfo(IChannel channel, Message message)
        {
            STipInfo msg = (STipInfo)message;
            MessageBox.Show(msg.info);
        }
    }
}
