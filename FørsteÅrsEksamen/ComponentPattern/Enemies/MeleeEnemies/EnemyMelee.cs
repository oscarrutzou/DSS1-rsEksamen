namespace DoctorsDungeon.ComponentPattern.Enemies.MeleeEnemies;

//Asser

public abstract class EnemyMelee : Enemy
{
    protected EnemyMelee(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Update()
    {
        base.Update();
    }
}