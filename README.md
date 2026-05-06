<a id="readme-top"></a>

<div align="center">
  
[![Commits][commits-shield]][commits-url]
[![Issues][issues-shield]][issues-url]

</div>

<br />
<div align="center">
  <a href="https://github.com/lienkko/GhostHouse">
    <img src="images/Enchanted_Book.gif" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">Ghost House</h3>

  <p align="center">
    2D игра в жанре Roguelike с элементами хоррора
    <br />
  </p>
  <h3><a href="https://github.com/lienkko/GhostHouse/releases/tag/v0.1.4">Скачать последнюю версию игры - v0.1.4</a></h3>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Содержание</summary>
  <ol>
    <li>
      <a href="#о-проекте">О проекте</a>
      <ul>
        <li><a href="#идея">Идея</a></li>
        <li><a href="#цель">Цель</a></li>
        <li><a href="#используемые-ресурсы">Используемые ресурсы</a></li>
      </ul>
    </li>
    <li>
      <a href="#инструкция-по-прохождению">Инструкция по прохождению</a>
      <ul>
        <li><a href="#управление">Управление</a></li>
        <li><a href="#подсказки">Подсказки</a></li>
        <li><a href="#читы">Читы</a></li>
      </ul>
    </li>
    <li><a href="#задачи">Задачи</a></li>
    <li><a href="#участники">Участники</a></li>
    <li><a href="#ps">P.S.</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## О проекте

[![Logo Screen Shot][logo-screenshot]](https://github.com/lienkko/GhostHouse)
### Идея
Главный герой оказывается в отеле, где единственный шанс выбраться, это пройти все двери до 100. По пути ему встречаются разные враждебные существа. Его главные оружия это ум и ловкость, которыми придется пользоваться на протяжении всего прохождения.

### Цель
Наша цель заключается в том, чтобы сделать увлекательную игру с множеством оригинальных головоломок, которая будет пользоваться популярностью среди жюри ПД и остальных студентов мехмата (а может даже за пределами мехмата 😉) .

### Используемые ресурсы

* [![Unity][Unity]][unity-url]
* [![C#][CSharp]][csharp-url]

<!-- GuideMap -->
## Инструкции по прохождению

### Управление

* Ходьба
  <br />
  <a>
    <img src="images/WASD.png" alt="Walk" width="93" height="64">
  </a>
* Красться
  <br />
  <a>
    <img src="images/SHIFT.png" alt="Crouch" width="74" height="31">
  </a>
* Взаимодействовать
  <br />
  <a>
    <img src="images/E.png" alt="UseHide" width="29" height="31">
  </a>
  
* Подобрать<br />
<a><img src="images/F.png" alt="Take" width="29" height="31"></a>
* Выбросить<br />
<a><img src="images/G.png" alt="Drop" width="29" height="31"></a>
  
### Подсказки
<br />
  <a>
    <img src="images/Desert.png" alt="Desert" width="675" height="134">
  </a>


### Читы

1. Начать игру (в стартовой комнате)
   ```sh
   startgame
   ```
2. Включить/выключить режим бога
   ```sh
   godmode (1/0)
   ```
3. Призвать Wraith
   ```sh
   summon_wraith
   ```
4. Открыть сейф в комнате
   ```sh
   open_safe
   ```
5. Перезапустить игру
   ```sh
   restartgame
   ```
6. Переместиться в следующую комнату (обходя сейф)
   ```sh
   nextroom
   ```
7. Переместиться в предыдующую комнату
   ```sh
   prevroom
   ```
8. Включить/выключить свет в комнате
   ```sh
   room_lights (1/0)
   ```
9. Очистить окно чата
   ```sh
   clear
   ```   


<p align="right">(<a href="#readme-top">К началу</a>)</p>


<!-- ROADMAP -->
## Задачи

- [x] Реализованы основные механики передвижения
    - [x] Ходьба
    - [x] Приседание
- [x] Добавлена возможность прятаться
- [x] Добавлена генерация комнат
- [x] Реализовано перемещение между комнатами
- [x] Добавлено стартовое меню
- [x] Добавлено меню паузы
- [x] Добавлена консоль с читами
- [x] Добавлен инвентарь
- [ ] Добавлены все основные головоломки и испытания
- [x] Добавлены основные враждебные существа
- [ ] Добавлены боссы и реализована их логика
    - [x] Босс1
    - [ ] Босс2
    - [ ] Босс3
    - [ ] Босс4
- [ ] Добавлена финальная сцена

<p align="right">(<a href="#readme-top">К началу</a>)</p>



<!-- CONTRIBUTORS -->
## Участники
[![Conributors][contibutors-logo]](https://github.com/lienkko/GhostHouse/graphs/contributors)

<p align="right">(<a href="#readme-top">К началу</a>)</p>

<!-- POST SCRIPTUM -->
## P.S.
Нам очень важно ваше мнение по поводу проекта. Даже если вы жюри, не скупитесь на хороший совет😊. Мы будем рады любой идее и любой конструктивной критике в разделах discussions или issues.
И конечно не забывайте ставить 🌟звезды🌟

<p align="right">(<a href="#readme-top">К началу</a>)</p>



<!-- MARKDOWN LINKS & IMAGES -->
[commits-shield]: https://img.shields.io/github/commit-activity/t/lienkko/GhostHouse.svg?style=for-the-badge
[commits-url]: https://github.com/lienkko/GhostHouse/commits
[issues-shield]: https://img.shields.io/github/issues/lienkko/GhostHouse.svg?style=for-the-badge
[issues-url]: https://github.com/lienkko/GhostHouse/issues
[logo-screenshot]: images/Logo.png
[Unity]: https://img.shields.io/badge/Unity-000000?style=for-the-badge&logo=unity&logoSize=auto
[unity-url]: https://unity.com
[CSharp]: https://img.shields.io/badge/C%23-000000?style=for-the-badge&logo=dotnet&logoSize=auto
[csharp-url]: https://learn.microsoft.com/ru-ru/dotnet/csharp/
[contibutors-logo]: https://contrib.rocks/image?repo=lienkko/GhostHouse 
