using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.CommandPattern.Commands;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Classes;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.ComponentPattern.Weapons;
using FørsteÅrsEksamen.ComponentPattern.Weapons.RangedWeapons;
using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.Other;
using FørsteÅrsEksamen.RepositoryPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace FørsteÅrsEksamen.GameManagement.Scenes
{
    public class ErikTestScene : Scene
    {
        private GameObject weapon;
        private GameObject bow;
        private Spawner spawner;
        private Player player;
        private GameObject spawnerGameObject;
        private Grid grid;
        

        public override void Initialize()
        {
            SetLevelBG();
            StartGrid();
            PlayerSpawnPos = new Point(5, 5);
            MakePlayer();

            
            OnPlayerChanged();

            InitSpawner();
            

            MakeWeapon();
            GameWorld.Instance.WorldCam.position = Vector2.Zero;
            
            AttackCommand();
            //spawner = new Spawner(new GameObject(), player);
            //GameWorld.Instance.Instantiate(spawner.GameObject);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
        }

        private void InitSpawner()
        {
            spawnerGameObject = new GameObject();
            Spawner spawner = spawnerGameObject.AddComponent<Spawner>();
            spawner.InitializeSpawner(PlayerGo, GridManager.Instance.CurrentGrid);
            GameWorld.Instance.Instantiate(spawnerGameObject);
        }

        private void SetLevelBG()
        {
            GameObject go = new();
            go.Type = GameObjectTypes.Background;
            go.Transform.Scale = new(4, 4);

            SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.SetSprite(TextureNames.TestLevel);
            spriteRenderer.IsCentered = false;

            GameWorld.Instance.Instantiate(go);
        }

        private void MakePlayer()
        {
            PlayerGo = PlayerFactory.Create(ClassTypes.Warrior, WeaponTypes.Sword);
            PlayerGo.Transform.Position = GridManager.Instance.CurrentGrid.Cells[PlayerSpawnPos].Transform.Position;
            PlayerGo.Transform.GridPosition = PlayerSpawnPos;
            GameWorld.Instance.WorldCam.position = PlayerGo.Transform.Position;
            GameWorld.Instance.Instantiate(PlayerGo);
        }

        private void StartGrid()
        {
            GameObject gridGo = new();
            Grid grid = gridGo.AddComponent<Grid>("Test1", new Vector2(0, 0), 24, 18);
            grid.GenerateGrid();
            GridManager.Instance.SaveGrid(grid);
        }

        private void SetCommands()
        {
            player = PlayerGo.GetComponent<Player>();
            
            InputHandler.Instance.AddKeyUpdateCommand(Keys.D, new MoveCmd(player, new Vector2(1, 0)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.A, new MoveCmd(player, new Vector2(-1, 0)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.W, new MoveCmd(player, new Vector2(0, -1)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.S, new MoveCmd(player, new Vector2(0, 1)));

            InputHandler.Instance.AddKeyButtonDownCommand(Keys.D1, new CustomCmd(player.UseItem));
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Tab, new CustomCmd(() => { GridManager.Instance.ShowHideGrid(); }));
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Space, new CustomCmd(Attack));
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.O, new CustomCmd(() => { DBGrid.SaveGrid(GridManager.Instance.CurrentGrid); }));
        }

        public override void OnPlayerChanged()
        {
            InputHandler.Instance.RemoveAllExeptBaseCommands();
            SetCommands();
        }

        private void MakeWeapon()
        {
            weapon = WeaponFactory.Create(WeaponTypes.Sword);
            
            bow = WeaponFactory.Create(WeaponTypes.Bow);
            
            
            GameWorld.Instance.Instantiate(weapon);
            GameWorld.Instance.Instantiate(bow);
        }

       

        private void Attack()
        {
            weapon.GetComponent<Weapon>().Attack();
            
            //projectile.GetComponent<MagicStaff>().Attack();
        }

        private void AttackCommand()
        {
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Space,
                new CustomCmd(Attack));

            InputHandler.Instance.AddKeyButtonDownCommand(Keys.B,
                new CustomCmd(Shoot));
        }

        public void Shoot()
        {
           var rangedWeapon = bow.GetComponent<RangedWeapon>();  

            //projectile.GetComponent<Projectile>().SetValues(MathHelper.Pi);

            if (rangedWeapon != null)
            {

                rangedWeapon.Shoot();
                

            }

        }


        public override void DrawInWorld(SpriteBatch spriteBatch)
        {
            base.DrawInWorld(spriteBatch);

            //spriteBatch.Draw(GlobalTextures.Textures[TextureNames.WoodSword], Vector2.Zero, Color.White);
        }

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            base.DrawOnScreen(spriteBatch);
        }
    }
}