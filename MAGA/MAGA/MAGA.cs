using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

public class MAGA : PhysicsGame
{
    private double kaantymisnopeus = 55000; //Nopeus, jolla pelaaja kääntyy
    public PhysicsObject trump; //pelaaja
    PhysicsObject meksikolaisKolo; // Paikka josta meksikolaiset syntyvät
    int tiilet = 0; //tiilet, jotka pelaajalla on mukana
    int laasti = 0; //laastin määrä, joka pelaajalla on mukana

    public override void Begin()
    {
        //Luodaan pelin kenttä txt tiedostosta
        TileMap ruudut = TileMap.FromLevelAsset("MAGA");
        ruudut.SetTileMethod('J', LuoMuuriPohja);
        ruudut.SetTileMethod('M', LuoMeksikolaisKolo);
        ruudut.SetTileMethod('T', LuoTrump);
        ruudut.SetTileMethod('%', LuoTiilipino);
        ruudut.SetTileMethod('L', LuoLaasti);
        ruudut.SetTileMethod('A', LuoVapaaMaa);
        // TODO: Refaktoroi nämä alilohjelmat järkevämmäksi
        ruudut.Execute();

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Left, ButtonState.Down, KaannaVasemmalle, "Käännä vasemmalle");
        Keyboard.Listen(Key.Left, ButtonState.Released, LopetaPyoriminen, "Lopettaa pyörimisen");
        Keyboard.Listen(Key.Right, ButtonState.Down, KaannaOikealle, "Käännä oikealle");
        Keyboard.Listen(Key.Right, ButtonState.Released, LopetaPyoriminen, "Lopettaa pyörimisen");
        Keyboard.Listen(Key.Down, ButtonState.Down, Peruuta, "Peruuttaa");
        Keyboard.Listen(Key.Down, ButtonState.Released, Pysahdy, "Pysähtyy");
        Keyboard.Listen(Key.Up, ButtonState.Down, Liikuta, "Liikkuu eteenpäin");
        Keyboard.Listen(Key.Up, ButtonState.Released, Pysahdy, "Pysähtyy");
        Keyboard.Listen(Key.A, ButtonState.Released, HeitaTiili, "Heitä meksikolaista tiilellä");


    }

    void RakennaMuuri()
    {
        if (laasti > 0 && tiilet > 0)
        {
            //TODO Tee muurin pohjat ja käsittely niiden rakentamiselle
        }
    }
    /// <summary>
    /// Aliohjelma, joka käsittelee tiilen heittämisen
    /// </summary>
    private void HeitaTiili()
    {
        if(tiilet > 0)
        {
            LuoLentavaTiili(trump.Position);
        }
    }
    /// <summary>
    /// Luodaan lentävä tiili ja annetaan sille nopeus
    /// </summary>
    /// <param name="paikka">sijainti, johon tiili luodaan, tässä kohtaa aina pelaajan päälle</param>
    private void LuoLentavaTiili(Vector paikka)
    {
        PhysicsObject tiili = PhysicsObject.CreateStaticObject(20, 40);
        tiili.Position = paikka;
        tiili.Shape = Shape.Rectangle;
        tiili.Color = Color.Orange;
        tiili.CollisionIgnoreGroup = 1;
        tiili.Tag = "tiili";
        Vector nokanSuunta = Vector.FromLengthAndAngle(1500, trump.Angle);
        tiili.Velocity = nokanSuunta;
        tiili.AngularVelocity = 100;
        tiili.CollisionIgnoreGroup = 1;
        Add(tiili);
    }
    /// <summary>
    /// Käsitellään tiilten hakeminen pinosta
    /// </summary>
    /// <param name="collidingObject">tiilipino</param>
    /// <param name="otherObject">Trump, pelaaja</param>
    private void HaeTiili(IPhysicsObject collidingObject, IPhysicsObject otherObject)
    {
        tiilet = 2;
    }
    /// <summary>
    /// Käsitellään laastin noutaminen
    /// </summary>
    /// <param name="collidingObject">laasti</param>
    /// <param name="otherObject">Trump, pelaaja</param>
    void HaeLaasti(IPhysicsObject collidingObject, IPhysicsObject otherObject)
    {
        laasti = 5;
    }
    /// <summary>
    /// Aliohjelma, joka käsittelee pelaajan pysähtymisen x ja y suunnassa
    /// </summary>
    private void Pysahdy()
    {
        trump.StopHorizontal();
        trump.StopVertical();
    }
    /// <summary>
    /// Aliohjelma, joka käsittelee pelaajan pyörimisen
    /// </summary>
    private void LopetaPyoriminen()
    {
        trump.StopAngular();
    }
    /// <summary>
    /// pelaajan kääntyminen oikealle
    /// </summary>
    private void KaannaOikealle()
    {
        trump.ApplyTorque(-kaantymisnopeus);
    }
    /// <summary>
    /// pelaajan kääntyminen vasemmalle TODO: refaktoroi
    /// </summary>
    private void KaannaVasemmalle()
    {
        trump.ApplyTorque(kaantymisnopeus);
    }
    /// <summary>
    /// Käsittelee pakittamisen, joka on puolet normaalista kävelynopeudesta
    /// </summary>
    private void Peruuta()
    {
        Vector nokanSuunta = Vector.FromLengthAndAngle(500, trump.Angle);
        trump.Velocity = -nokanSuunta / 2;
    }
    /// <summary>
    /// Käsittelee pelaajan liikkumisen eteenpäin
    /// </summary>
    void Liikuta()
    {
        Vector nokanSuunta = Vector.FromLengthAndAngle(500, trump.Angle);
        trump.Velocity = nokanSuunta;
    }
    /// <summary>
    /// Luodaan laastin hakupaikka
    /// </summary>
    /// <param name="paikka">sijainti johon laasti luodaan</param>
    /// <param name="leveys"></param>
    /// <param name="korkeus"></param>
    void LuoLaasti(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject laasti = PhysicsObject.CreateStaticObject(leveys, korkeus);
        laasti.Position = paikka;
        laasti.Shape = Shape.Rectangle;
        laasti.Color = Color.Gray;
        laasti.Tag = "laasti";
        AddCollisionHandler(laasti, "Trump", HaeLaasti);
        Add(laasti);
    }
    /// <summary>
    /// Luodaan tiilipino
    /// </summary>
    /// <param name="paikka"></param>
    /// <param name="leveys"></param>
    /// <param name="korkeus"></param>
    void LuoTiilipino(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject tiilipino = PhysicsObject.CreateStaticObject(leveys, korkeus);
        tiilipino.Position = paikka;
        tiilipino.Shape = Shape.Rectangle;
        tiilipino.Color = Color.Orange;
        tiilipino.Tag = "tiili";
        AddCollisionHandler(tiilipino, "Trump", HaeTiili);
        Add(tiilipino);
    }
    /// <summary>
    /// Luodaan pelaaja
    /// </summary>
    /// <param name="paikka"></param>
    /// <param name="leveys"></param>
    /// <param name="korkeus"></param>
    void LuoTrump(Vector paikka, double leveys, double korkeus)
    {
        trump = new PhysicsObject(leveys, korkeus);
        trump.Position = paikka;
        trump.Shape = Shape.Rectangle;
        trump.Color = Color.White;
        trump.MaxAngularVelocity = 5;
        trump.CollisionIgnoreGroup = 1;
        trump.Tag = "Trump";
        //trump.Image = trumpKuva;
        Add(trump);
    }
    /// <summary>
    /// Luodaan paikka, josta meksikolaiset syntyvät
    /// </summary>
    /// <param name="paikka"></param>
    /// <param name="leveys"></param>
    /// <param name="korkeus"></param>
    void LuoMeksikolaisKolo(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject meksikolaisKolo = PhysicsObject.CreateStaticObject(leveys, korkeus);
        meksikolaisKolo.Position = paikka;
        meksikolaisKolo.Shape = Shape.Rectangle;
        meksikolaisKolo.Color = Color.Yellow;
        Add(meksikolaisKolo);
    }
    /// <summary>
    /// Luodaan pohja, johon muuri pitää luoda
    /// </summary>
    /// <param name="paikka"></param>
    /// <param name="leveys"></param>
    /// <param name="korkeus"></param>
    void LuoMuuriPohja(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject muuripohja = PhysicsObject.CreateStaticObject(leveys, korkeus);
        muuripohja.Position = paikka;
        muuripohja.Shape = Shape.Rectangle;
        muuripohja.Color = Color.Blue;
        muuripohja.Tag = "muuri";
        Add(muuripohja);
    }
    /// <summary>
    /// Luodaan paikka, jonne meksikolaiset juoksevat
    /// </summary>
    /// <param name="paikka"></param>
    /// <param name="leveys"></param>
    /// <param name="korkeus"></param>
    void LuoVapaaMaa(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject vapaaMaa = PhysicsObject.CreateStaticObject(leveys, korkeus);
        vapaaMaa.Position = paikka;
        vapaaMaa.Shape = Shape.Rectangle;
        vapaaMaa.Color = Color.Blue;
        vapaaMaa.Tag = "vapaus";
        Add(vapaaMaa);
    }

}
