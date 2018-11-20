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
            register.Register(Command.C_PLAYER_MOVE, OnRecvPlayerMove);
            register.Register(Command.C_PLAYER_JUMP, OnRecvPlayerJump);
            register.Register(Command.C_PLAYER_ATTACK, OnRecvPlayerAttack);
            register.Register(Command.C_PLAYER_TAKE, OnRecvPlayerTake);
            register.Register(Command.C_POSITION_REVISE, OnRecvPositionRevise);
            register.Register(Command.C_ENEMY_CLOSING, OnRecvEnemyClosing);
            register.Register(Command.C_DAMAGE, OnRecvDamage);

            // DEBUG ..
            register.Register(Command.C_FIND_PATH, OnRecvFindPath);

        }


        static void ClientTipInfo(IChannel channel, string info)
        {
            STipInfo tipInfo = new STipInfo();
            tipInfo.info = info;
            channel.Send(tipInfo);
        }





    }
}
