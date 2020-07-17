using System;
using System.Collections.Generic;

class MainClass {
  public static void Main (string[] args) {

    Room currentRoom = new Room();

    do
    {
      if(currentRoom.ended) currentRoom.NextRoom();
      currentRoom.Action();      
    } while (Player.hp > 0);
    Console.WriteLine("Вы сдохли!");
    Console.ReadLine();
  }
}

static class Player
{
  public static int hp = 100;
  public static int dmg = 20;
  public static int potionCount = 2;
  static int maxhp = 100;

  static public void Action()
  {
    Console.WriteLine("1 - Атаковать");
    Console.WriteLine("2 - Пить зелье здоровья (+100HP)");
    string action = "0";
    do
    {
      action = Console.ReadLine();
      switch (action)
      {
        case "1":
          //Игрок ввёл красное
          Attack();
          break;
        case "2":
          TakeHealPotion();
          Status();
          break;
        default:
          action = "0";
          break;
      };
    } while (action == "0");
  }

  public static void Status()
  {
    Console.WriteLine ($"________________");
    Console.WriteLine ($"Ваше HP: {Player.hp} HP; Ваш урон: {Player.dmg}; Зелий: {Player.potionCount}");
    Console.WriteLine ($"________________");
  }
  static void Attack()
  {

  }
  static void TakeHealPotion()
  {    
    potionCount--;
    int healed = Heal(100);
    Console.WriteLine ($"Вы выпили зелье здоровья +{healed} HP");
  }

  static int Heal(int _hp)
  {
    int oldhp = hp;
    hp += _hp;
    if(hp > maxhp) hp = maxhp;
    return hp - oldhp;
  }
}

interface IEnemy
{
  void TakeDmg(int _dmg);
  void Attack();
}

class Troll:IEnemy
{
  int hp;
  int dmg;

  public Troll()
  {
    hp = 100;
    dmg = 20;
    Console.WriteLine($"Из комментов на вас выпрыгнул тролль (HP: {hp} DMG: {dmg})");
  }

  public void TakeDmg(int _dmg)
  {
    hp -= dmg;
  }

  public void Attack()
  {
    Console.WriteLine($"Вас атаковал тролль(HP: {hp}): -{dmg} HP");
    Player.hp -= dmg;
  }
}

class Room
{
  static Random rand = new Random();
  public bool ended;
  List<IEnemy> enemies;
  
  public Room()
  {
    ended = true;
    enemies = new List<IEnemy>();
  }

  public void NextRoom()
  {
    ended = false;

    CreateEnemy(rand.Next(1, 2));
  }

  void CreateEnemy(int count)
  {
    for(int i = 0; i < count; i++)
    {
      enemies.Add(new Troll());
    }    
  }

  public void Action()
  {
    foreach(IEnemy enemy in enemies)
    {
      enemy.Attack();
    }
    Player.Status();
    Player.Action();
  }
}