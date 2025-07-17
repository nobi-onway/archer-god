using Netick.Unity;

public class BaseNetworkBehaviour : NetworkBehaviour
{
    public override void NetworkFixedUpdate()
    {
        Tick();
    }
    
    protected virtual void Tick() { }
}