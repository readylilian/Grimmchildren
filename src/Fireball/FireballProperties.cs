using Fisobs.Properties;
namespace SlugTemplate
{
    internal class FireballProperties : ItemProperties
{
    public override void Throwable(Player player, ref bool throwable)
        => throwable = true;
    public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 0;

    public override void ScavWeaponPickupScore(Scavenger scav, ref int score)
        => score = 0;

    // Don't throw shields
    public override void ScavWeaponUseScore(Scavenger scav, ref int score)
        => score = 0;
    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
    {
        grabability = Player.ObjectGrabability.OneHand;
    }
}
}