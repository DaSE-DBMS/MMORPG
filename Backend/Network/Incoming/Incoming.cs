using Common;

namespace Backend.Network
{
    public partial class Incoming
    {
        IRegister register;
        public Incoming(IRegister register)
        {
            this.register = register;
            RegisterAll();
        }

        private void RegisterAll()
        {
            register.Register(Command.C_LOGIN, OnRecvLogin);
            register.Register(Command.C_REGISTER, OnRecvRegister);
            register.Register(Command.C_PLAYER_ENTER, OnRecvPlayerEnter);
            register.Register(Command.C_PLAYER_MOVE, RecvPlayerMove);
            register.Register(Command.C_PLAYER_JUMP, RecvPlayerJump);
            register.Register(Command.C_PLAYER_ATTACK, RecvPlayerAttack);
            register.Register(Command.C_PLAYER_TAKE, RecvPlayerTake);
            register.Register(Command.C_POSITION_REVISE, RecvPositionRevise);

            // DEBUG ..
            register.Register(Command.C_FIND_PATH, RecvFindPath);

        }


        static void ClientTipInfo(IChannel channel, string info)
        {
            STipInfo tipInfo = new STipInfo();
            tipInfo.info = info;
            channel.Send(tipInfo);
        }





    }
}
