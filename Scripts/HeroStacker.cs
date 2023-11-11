using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class HeroStacker : VBoxContainer
    {
        public static HeroStacker Instance { get; private set; }

        public override void _Ready()
        {
            Instance = this;
        }
        public void StackTokens()
        {
            SelectToken selectToken = (SelectToken)Globals.Instance.selectedToken;
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(selectToken.countyPopulation.location);
            List<CountyPopulation> heroList = selectCounty.countyData.heroCountyPopulation;

            /*
            if (heroList.Count > 1)
            {
                spawnedTokenList[0].GetComponent<TokenInfo>().counterGameObject.SetActive(true);

                for (int i = 0; i < spawnedTokenList.Count(); i++)
                {
                    TokenInfo tokenInfo = spawnedTokenList[i].GetComponent<TokenInfo>();
                    GameObject tokenLocation = tokenInfo.countyPopulation.location;

                    // Change each token's order to be lower then the one on "top" of it.
                    tokenInfo.OrderInLayer = 100 - i;
                    tokenInfo.counterText.text = spawnedTokenList.Count().ToString();

                    if (i == 0)
                    {
                        spawnedTokenList[i].GetComponentInChildren<TokenInfo>().nameGameObject.SetActive(true);

                        spawnedTokenList[i].transform.position = tokenLocation.GetComponent<CountyInfo>().tokenSpawn.transform.position;

                        if (WorldMapLoad.Instance.CurrentlySelectedToken.GetComponent<TokenMovement>().Move == false)
                        {
                            WorldMapLoad.Instance.CurrentlySelectedToken = spawnedTokenList[i];
                        }

                    }
                    else
                    {
                        spawnedTokenList[i].GetComponentInChildren<TokenInfo>().nameGameObject.SetActive(false);
                        spawnedTokenList[i].GetComponent<TokenInfo>().counterGameObject.SetActive(false);
                        spawnedTokenList[i].transform.position
                            = new Vector2(tokenLocation.GetComponent<CountyInfo>().tokenSpawn.transform.position.x + (i * 0.1f)
                            , tokenLocation.GetComponent<CountyInfo>().tokenSpawn.transform.position.y);
                    }
                }
            }
            */
        }
    }
}
