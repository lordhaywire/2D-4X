using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class HeroStacker : Node2D
    {
        //public static HeroStacker Instance { get; private set; }

        public ListWithNotify<CharacterBody2D> spawnedTokenList = new(); // This is not a normal list.
        public override void _Ready()
        {
            //Instance = this;
            spawnedTokenList.notifyListeners += ListChanged;
        }

        private void ListChanged()
        {
            //Debug.Log("Something got added to the list.");
            StackTokens();
        }
        public void StackTokens()
        {
            if (spawnedTokenList.Count() > 1)
            {
                for (int i = 0; i < spawnedTokenList.Count(); i++)
                {
                    SelectToken selectToken = (SelectToken)spawnedTokenList[i];
                    SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(selectToken.countyPopulation.location);
                    GD.Print("Hero in stack: " + selectToken.Name + i);
                    // Change each token's order to be lower then the one on "top" of it.
                    //selectToken.ZIndex = 100 - i; // I don't think this matters and instead we need the order in the tree to be the same
                    // as the list.

                    selectToken.stackCountLabel.Text = spawnedTokenList.Count().ToString();

                    if (i == 0)
                    {
                        selectToken.tokenNameLabel.Show();
                        selectToken.stackCountLabel.Show();
                        selectToken.GlobalPosition = selectCounty.heroSpawn.GlobalPosition;

                        /*
                        if (Globals.Instance.selectedToken.TokenMovem == false)
                        {
                            WorldMapLoad.Instance.CurrentlySelectedToken = spawnedTokenList[i];
                        }
                        */
                    }
                    else
                    {
                        selectToken.stackCountLabel.Hide();
                        selectToken.tokenNameLabel.Hide();
                        selectToken.GlobalPosition
                            = new Vector2(selectCounty.heroSpawn.GlobalPosition.X + (i * Globals.Instance.heroStackingOffset)
                            , selectCounty.heroSpawn.GlobalPosition.Y);
                    }
                    selectCounty.heroSpawn.MoveChild(selectToken, );
                }
            }
        }

        public class ListWithNotify<T> where T : class
        {

            readonly List<T> list = new();
            public Action notifyListeners;

            // This makes this function like a normal list.
            public T this[int i]
            {
                get
                {
                    if (list.Count != 0)
                    {
                        return list[i];
                    }
                    else
                    {
                        GD.Print("Default: " + default(T));
                        return null;
                    }
                }
                set { list[i] = value; }
            }

            // These make the make the normal list actions work correctly. If we need to do other things to the list we need to add them here.
            public int Count() // This is nor a method so when using it, it will need ().
            {
                return list.Count; // I changed this from Count().
            }
            public void Add(T item)
            {
                list.Add(item);
                notifyListeners?.Invoke();
            }

            public void Insert(int i, T item)
            {
                list.Insert(i, item);
                notifyListeners?.Invoke();
            }

            public void Remove(T item)
            {
                list.Remove(item);
                notifyListeners?.Invoke();
            }

            public void RemoveAt(int i)
            {
                list.RemoveAt(i);
                notifyListeners?.Invoke();
            }

        }
    }
}


