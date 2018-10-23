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
            register.Register(Command.C_PLAYER_ENTER, OnRecvPlayerEnter);
            register.Register(Command.C_PLAYER_MOVE, RecvPlayerMove);
            register.Register(Command.C_PLAYER_JUMP, RecvPlayerJump);
            register.Register(Command.C_PLAYER_ATTACK, RecvPlayerAttack);
            register.Register(Command.C_PLAYER_TAKE, RecvPlayerTake);


            // DEBUG ..
            register.Register(Command.C_FIND_PATH, RecvFindPath);

        }








    }
}
