using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvTipInfo(IChannel channel, Message message)
        {
            STipInfo msg = message as STipInfo;
            MessageBox.Show(msg.info);
        }
    }
}
