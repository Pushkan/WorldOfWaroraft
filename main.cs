using System;
using System.Collections.Generic;
using System.Linq;

class MainClass 
{
  public static void Main (string[] args) {

    Room.NextRoom();
    Room.Action();      
    Console.WriteLine ($"________________");
    Console.WriteLine($"Вы мертвы! Спасибо за игру. Ваш итоговый счёт: {Player.score}. Вы дошли до {Room.numberRoom} комнаты.");
    Console.ReadLine();
  }
}

static class Player
{
  public static int hp = 100;
  public static int dmg = 30;
  public static int potionCount = 2;
  public static int score = 0;
  static int maxhp = 100;
  static int xp = 0;
  public static int lvl = 1;
  public static int kpotion = 100;

  static public void Action()
  {
    Console.WriteLine ($"________________");
    Console.WriteLine("1 - Атаковать");
    Console.WriteLine($"2 - Пить зелье здоровья (+{kpotion}HP)");
    string action = "0";
    do
    {
      action = Console.ReadLine();
      switch (action)
      {
        case "1":
          Attack();
          break;
        case "2":
          TakeHealPotion();
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
    double k = xp / (60 * Math.Pow(1.3, lvl));
    string currentxp = String.Format("{0:0.##}", 100 * (k));
    Console.WriteLine ($"Вы - LVL: {lvl}({currentxp}%) HP: {Player.hp}/{maxhp}; DMG: {Player.dmg}; Зелий: {Player.potionCount}");
  }

  static void Attack()
  {
    string choose = "";
    int chooseInt = 0;
    do{
      Console.WriteLine ($"________________");
      Console.WriteLine("Выберите врага, которого хотите атаковать");
      Room.ShowEnemyList();
      choose = Console.ReadLine();
      bool success = Int32.TryParse(choose, out chooseInt);
         if (success)
         {
           //Проверяем, входит ли номер в наш список врагов
           if(!Room.enemies.ContainsKey(chooseInt))
           {
             Console.WriteLine ($"________________");
             Console.WriteLine($"Врага с номером {choose} не найдено");
             chooseInt = 0;
           }
         }
    } while (chooseInt == 0);
    Console.Clear();
    Enemy enemy = Room.enemies[chooseInt];
    Console.WriteLine ($"________________");
    Console.WriteLine($"Вы атаковали {enemy.name} №{choose} {enemy.hp} HP - {enemy.TakeDmg(dmg)} HP = {enemy.hp} HP");
    if(enemy.hp <= 0)
    {
      Console.WriteLine($"{enemy.name} №{choose} умер. +{enemy.xp} XP");
      GetXP(enemy.xp);
    }
    

  }

  static public void TakeHealPotion()
  {    
    Console.Clear();
    if(potionCount <= 0)
    {
      Console.WriteLine ($"________________");
      Console.WriteLine ($"Вы потянулись за очередным зельем, но ничего не обнаружили. Кажется, у вас кончились зелья.");
      return;
    }
    potionCount--;
    int healed = Heal(kpotion);    
    Console.WriteLine ($"________________");
    Console.WriteLine ($"Вы выпили зелье здоровья +{healed} HP");
    Status();
  }

  static public void GiveHealPotion(int _count)
  {
    potionCount += _count;
  }

  static int Heal(int _hp)
  {
    int oldhp = hp;
    hp += _hp;
    if(hp > maxhp) hp = maxhp;
    return hp - oldhp;
  }

  static public void GetXP(int _xp)
  {
    xp += _xp;
    score += _xp;
    double nextlvlxp = 60 * Math.Pow(1.3, lvl);
    if(xp >= nextlvlxp)
    {
      xp -= System.Convert.ToInt32(nextlvlxp);
      LvlUp();
    }
  }

  static void LvlUp()
  {
    Console.WriteLine ($"________________");
    Console.WriteLine ($"Вы повысили свой уровень до {++lvl}");
    hp = maxhp;
    kpotion = System.Convert.ToInt32(Math.Pow(1.1, Player.lvl) * 100);
    Status();
    Console.WriteLine ($"________________");
    string choose = "";
    int chooseInt = 0;
    do
    {      
      Console.WriteLine ($"Выберите, что улучшить");
      Console.WriteLine ($"1. - MAX HP +10");
      Console.WriteLine ($"2. - DMG +10");
      choose = Console.ReadLine();
      bool success = Int32.TryParse(choose, out chooseInt);
      if(success)
      {
        if(!(chooseInt == 1 || chooseInt == 2))
        {
          chooseInt = 0;
        }
      }
    } while (chooseInt == 0);
    Console.Clear();
    if(chooseInt == 1)
    {
      maxhp += 10;
      Console.WriteLine ($"Вы увеличили максимальное HP");
    }
    else
    {
      dmg += 10;
      Console.WriteLine ($"Вы увеличили DMG");
    }
    hp = maxhp;
  }
}

abstract class Enemy
{
    public int hp;
    public int dmg;
    public string name;
    public int xp;
    public int lvl;

  virtual public int TakeDmg(int _dmg)
  {
    int oldhp = hp;
    hp -= _dmg;
    if(hp <= 0) Die();
    return oldhp - hp;
  }

  virtual public void Attack()
  {
    Console.WriteLine($"Вас атаковал {name} {lvl} LVL (HP: {hp}): -{dmg} HP");
    Player.hp -= dmg;
  }

  virtual public void StartStatus()
  {
    Console.WriteLine($"Из комментов на вас выпрыгнул {name} {lvl} LVL (HP: {hp} DMG: {dmg})");
  }

  void Die()
  {
    var item = Room.enemies.First(kvp => kvp.Value == this);
    Room.enemies.Remove(item.Key);    
  }
}

static class Room
{
  static public Random rand = new Random();
  static public bool ended = true;
  static public Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();
  static public int numberRoom = 0;
  static bool chestClosed = true;

  static public void NextRoom()
  {
    chestClosed = true;
    Console.Clear();
    Console.WriteLine ($"________________");
    Console.WriteLine ($"Вы зашли в следующую комнату №{++numberRoom}");
    ended = false;
    CreateEnemy();
  }

  static void CreateEnemy()
  {
    int k1 = System.Convert.ToInt32(Math.Pow(1.3, Player.lvl));
    int k2 = System.Convert.ToInt32(Math.Pow(1.5, Player.lvl)) + 1;
    int count = rand.Next(k1, k2);
    Console.WriteLine ($"________________");
    int lvl = 1;
    for(int i = 1; i <= count; i++)
    {
      k1 = rand.Next(1,11);
      if(k1 <= 2){
        lvl = Player.lvl == 1 ? 1 :Player.lvl - 1;
      }
      else if(k1 <= 7)
      {
        lvl = Player.lvl;
      }
      else
      {
        lvl = Player.lvl + 1;
      }  
      Enemy enemy = new Troll(lvl);
      enemies.Add(i, enemy);
    }    
  }

  static public void Action()
  {
    do
    {
      Console.WriteLine ($"________________");
      foreach(KeyValuePair<int, Enemy>  kvpenemy in enemies)
      {
        kvpenemy.Value.Attack();
      }
      Player.Status();
      if(Player.hp > 0)
      {
        Player.Action();
      };
      if(enemies.Count() == 0)
      {
        if(ended) return;
        else EndRoom();      
      }
    } while (Player.hp > 0);
  }

  static public void EndRoom()
  {
    int xp = System.Convert.ToInt32(Math.Floor(10 * Math.Pow(1.3f, numberRoom)));;
    Player.GetXP(xp);
    if(chestClosed)
    {
      Console.WriteLine ($"________________");
      Console.WriteLine ($"Вы зачистили комнату. +{xp} XP");
    }
    Player.Status();    
    string action = "0";    
    do
    {      
      Console.WriteLine ($"________________");
      Console.WriteLine ($"1 - Идти в следующую комнату");
      Console.WriteLine ($"2 - Пить зелье здоровья (+{Player.kpotion}HP)");
      Console.WriteLine ($"3 - Открыть сундук");
      
      action = Console.ReadLine();
      switch (action)
      {
        case "1":
          NextRoom();
          break;
        case "2":
          Player.TakeHealPotion();
          action = "0";
          break;
        case "3":
          if(chestClosed)
          {
            chestClosed = false;
            CheckChest();            
          }
          else
          {
            Console.WriteLine ($"Сундук уже открыт");
          }
          action = "0";
          break;
        default:
          action = "0";
          break;
      };
    } while (action == "0" && (Player.hp > 0));
  }

  static void CheckChest()
  {
    int k = rand.Next(1, 11);
    Console.Clear();
    if(k < 7)
    {
      Console.WriteLine ($"________________");
      Console.WriteLine("Вы нашли 2 зелья здоровья");
      Player.GiveHealPotion(2);
      Player.Status();
    }
    else
    {
      Enemy enemy = new Mimic(Player.lvl);
      enemies.Add(1, enemy);
      Action();
    }
  }

  static public void ShowEnemyList()
  {
    Console.WriteLine();
    foreach(KeyValuePair<int, Enemy>  kvpenemy in enemies)
    {
      Console.WriteLine($"{kvpenemy.Key}. - {kvpenemy.Value.name} (HP: {kvpenemy.Value.hp} DMG: {kvpenemy.Value.dmg})");
    }
  }
}

class Troll:Enemy
{
  public Troll(int _lvl)
  {
    lvl = _lvl;
    float khp = Room.rand.Next(50, 62)/100f;
    float kdmg = Room.rand.Next(30, 42)/100f;
    xp = System.Convert.ToInt32(Math.Floor(40 * Math.Pow(1.2f, lvl)));
    
    hp = System.Convert.ToInt32(Math.Floor(khp * xp));
    dmg = System.Convert.ToInt32(Math.Floor(kdmg * xp));
    xp = System.Convert.ToInt32(Math.Floor(xp * 0.66));

    name = "Тролль";
    StartStatus();
  }
}

class Mimic:Enemy
{
  public Mimic(int _lvl)
  {
    lvl = _lvl;
    float khp = Room.rand.Next(100, 130)/100f;
    float kdmg = Room.rand.Next(40, 65)/100f;
    xp = System.Convert.ToInt32(Math.Floor(40 * Math.Pow(1.2f, lvl)));
    
    hp = System.Convert.ToInt32(Math.Floor(khp * xp));
    dmg = System.Convert.ToInt32(Math.Floor(kdmg * xp));
    xp = System.Convert.ToInt32(Math.Floor(xp * 0.2));

    name = "Мимик";
    StartStatus();
  }

  public override void StartStatus()
  {
    Console.WriteLine($"Ящик вдруг начал шевелиться и кусаться! Да это же {name} {lvl} LVL (HP: {hp} DMG: {dmg})");
  }
}