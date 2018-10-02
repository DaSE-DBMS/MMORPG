namespace Common
{
    public interface IRegister
    {
        void Register(Command command, MessageDelegate @delegate);
    }
}
