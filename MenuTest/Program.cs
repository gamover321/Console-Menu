using System;
using System.Collections.Generic;
using System.Linq;

namespace MenuTest
{
    class Program
    {

        public class MenuItem
        {
            public string Text { get; set; }
            public bool IsSelected { get; set; }

            public Menu NextLevelMenu { get; set; }
            public Menu PrevLevelMenu { get; set; }
        }

        public class Menu
        {

            private string NextLevelSeparator = "..";

            private int MaxCount => Items.Count;

            private int CurrentSelectedItemIndex { get; set; }

            public List<MenuItem> Items { get; set; }
            public MenuItemStyle SelectedItemStyle { get; set; }
            public MenuItemStyle NormalItemStyle { get; set; }

            public Menu()
            {
                CurrentSelectedItemIndex = -1; //Outside from menu
                Items = new List<MenuItem>();
                SelectedItemStyle = new MenuItemStyle();
                NormalItemStyle = new MenuItemStyle();
            }

            public void Draw()
            {
                Console.BackgroundColor = NormalItemStyle.BackgroundColor;
                Console.ForegroundColor = NormalItemStyle.ForegroundColor;
                Console.Clear();

                Console.WriteLine("Use arrow keys to navigate (up and down)");
                Console.WriteLine("");

                foreach (var menuItem in Items)
                {
                    if (menuItem.IsSelected)
                    {
                        Console.BackgroundColor = SelectedItemStyle.BackgroundColor;
                        Console.ForegroundColor = SelectedItemStyle.ForegroundColor;
                    }
                    else
                    {
                        Console.BackgroundColor = NormalItemStyle.BackgroundColor;
                        Console.ForegroundColor = NormalItemStyle.ForegroundColor;
                    }

                    Console.WriteLine(menuItem.Text);
                }
            }

            public bool SelectItem(int itemIndex)
            {
                if (itemIndex >= MaxCount)
                {
                    return false;
                }

                var oldItem = Items.ElementAtOrDefault(CurrentSelectedItemIndex);
                if (oldItem != null)
                {
                    oldItem.IsSelected = false;
                }

                var newItem = Items.ElementAtOrDefault(itemIndex);
                if (newItem == null)
                {
                    return false;
                }
                newItem.IsSelected = true;

                CurrentSelectedItemIndex = itemIndex;

                Draw();

                return true;
            }

            public bool SelectNext()
            {
                var index = CurrentSelectedItemIndex;
                index++;
                if (index >= MaxCount)
                {
                    return false;
                }

                return SelectItem(index);
            }

            public bool SelectPrev()
            {
                var index = CurrentSelectedItemIndex;
                index--;
                if (index >= MaxCount)
                {
                    return false;
                }

                return SelectItem(index);
            }

            public bool Choose()
            {

                var currentItem = Items.ElementAtOrDefault(CurrentSelectedItemIndex);
                if (currentItem == null)
                {
                    return false;
                }
                if (currentItem.Text == NextLevelSeparator)
                {
                    if (currentItem.PrevLevelMenu == null)
                    {
                        return false;
                    }
                    Items = new List<MenuItem>();
                    Items.AddRange(currentItem.PrevLevelMenu.Items);

                    CurrentSelectedItemIndex = currentItem.PrevLevelMenu.Items.FindIndex(i => i.IsSelected);
                }
                else
                {
                    if (currentItem.NextLevelMenu == null)
                    {
                        return false;
                    }

                    var oldItems = new List<MenuItem>();
                    oldItems.AddRange(Items);

                    Items = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Text = NextLevelSeparator,
                        IsSelected = true,
                        PrevLevelMenu =
                            new Menu
                            {
                                Items = oldItems,
                                CurrentSelectedItemIndex = CurrentSelectedItemIndex,
                                SelectedItemStyle = SelectedItemStyle
                            }
                    }
                };

                    Items.AddRange(currentItem.NextLevelMenu.Items);
                    CurrentSelectedItemIndex = 0;
                }
                
                
                
                
                

                Draw();
                return true;
            }
        }

        public class MenuItemStyle
        {
            public ConsoleColor BackgroundColor { get; set; }
            public ConsoleColor ForegroundColor { get; set; }
        }

        static void Main(string[] args)
        {

            var menu = new Menu
            {
                Items = new List<MenuItem>
                {
                    new MenuItem {Text = "A://"},
                    new MenuItem {Text = "C://", NextLevelMenu = new Menu
                    {
                        Items = new List<MenuItem>
                        {
                            new MenuItem {Text = "Program files", NextLevelMenu = new Menu
                            {
                                Items = new List<MenuItem>
                                {
                                    new MenuItem {Text = "Adobe", NextLevelMenu = new Menu
                                    {
                                        Items = new List<MenuItem>()
                                        {
                                            new MenuItem {Text = "readme.txt"}
                                        }
                                    } },
                                    new MenuItem {Text = "Microsoft", NextLevelMenu = new Menu
                                    {
                                        Items = new List<MenuItem>
                                        {
                                            new MenuItem {Text = "readme.txt"}
                                        }
                                    } }
                                }
                            } },
                            new MenuItem {Text = "boot.ini"}
                        },
                        SelectedItemStyle = new MenuItemStyle
                        {
                            BackgroundColor = ConsoleColor.Cyan,
                            ForegroundColor = ConsoleColor.Black
                        },
                        NormalItemStyle = new MenuItemStyle
                        {
                            BackgroundColor = ConsoleColor.Black,
                            ForegroundColor = ConsoleColor.White
                        },
                    }},
                    new MenuItem {Text = "D://"}
                },
                SelectedItemStyle = new MenuItemStyle
                {
                    BackgroundColor = ConsoleColor.Cyan,
                    ForegroundColor = ConsoleColor.Black
                },
                NormalItemStyle = new MenuItemStyle
                {
                    BackgroundColor = ConsoleColor.Black,
                    ForegroundColor = ConsoleColor.White
                },
            };


            menu.Draw();


            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            menu.SelectPrev();
                            break;
                        case ConsoleKey.DownArrow:
                            menu.SelectNext();
                            break;
                        case ConsoleKey.Enter:
                            menu.Choose();
                            break;
                    }
                }
            }

        }
    }
}
