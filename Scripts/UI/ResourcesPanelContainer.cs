using Godot;
using System;
using System.Linq;

namespace PlayerSpace
{
    public partial class ResourcesPanelContainer : PanelContainer
    {
        public static ResourcesPanelContainer Instance { get; private set; }

        [Export] private Label countyNameTitleLabel;
        [Export] private Label currentPerishableAvailableLabel;
        [Export] private Label maxPerishableAmountAvailableLabel;
        [Export] private Label currentNonperishableAvailableLabel;
        [Export] private Label maxNonperisableAmountAvailableLabel;

        [Export] public StorageHbox[] perishableResourceStorageHbox;
        [Export] public StorageHbox[] nonperishableResourceStorageHbox;

        public override void _Ready()
        {
           Instance = this;
        }

        private void OnVisibilityChanged()
        {
            if (Visible)
            {
                PlayerControls.Instance.playerControlsEnabled = false;
                UpdateMaxAmountLabels();
                UpdateResourceLabels();
            }
            else
            {
                PlayerControls.Instance.playerControlsEnabled = true;
            }
        }

        public void UpdateSpinBoxMaxValue(int amount, bool perishable, int index)
        {
            CountyData countyData = Globals.Instance.selectedLeftClickCounty.countyData;

            if(perishable)
            {
                for (int i = 0; i < perishableResourceStorageHbox.Length; i++)
                {
                    if(i != index)
                    {
                        GD.Print("Perishable Storage: " + countyData.perishableStorage);
                        GD.Print("Perishable Resources Length: " + countyData.perishableResources.Length);
                        
                        perishableResourceStorageHbox[i].maxAmountSpinBox.MaxValue 
                            = (countyData.perishableStorage / countyData.perishableResources.Length) 
                            + int.Parse(currentPerishableAvailableLabel.Text);
                        GD.Print(perishableResourceStorageHbox[i].maxAmountSpinBox.MaxValue);
                    }
                }
            }

        }
        public void UpdateAvailableStorage(int amount, bool perishable)
        {
            CountyData countyData = Globals.Instance.selectedLeftClickCounty.countyData;
            if (perishable)
            {
                currentPerishableAvailableLabel.Text
                    = (countyData.perishableStorage / countyData.perishableResources.Length - amount).ToString();
            }
            else
            {
                currentNonperishableAvailableLabel.Text
                = (countyData.nonperishableStorage / countyData.nonperishableResources.Length - amount).ToString();
            }
        }
        private void UpdateResourceLabels()
        {
            countyNameTitleLabel.Text = Globals.Instance.selectedLeftClickCounty.countyData.countyName;

            UpdateEachTypeOfResource(perishableResourceStorageHbox
                , Globals.Instance.selectedLeftClickCounty.countyData.perishableResources);
            UpdateEachTypeOfResource(nonperishableResourceStorageHbox
                , Globals.Instance.selectedLeftClickCounty.countyData.nonperishableResources);
        }

        private void UpdateEachTypeOfResource(StorageHbox[] storageHboxes, ResourceData[] resources)
        {
            for (int i = 0; i < resources.Length; i++)
            {
                storageHboxes[i].perishable = resources[i].perishable;
                storageHboxes[i].storageHboxIndex = i;
                storageHboxes[i].resourceNameLabel.Text
                    = $"{resources[i].resourceName}:";
                storageHboxes[i].resourceAmountLabel.Text
                    = resources[i].amount.ToString();
                GD.Print(resources[i].maxAmount);
                storageHboxes[i].resourceMaxAmountLabel.Text
                    = resources[i].maxAmount.ToString();


                // Spinbox
                if (resources[i].perishable)
                {
                    storageHboxes[i].maxAmountSpinBox.MaxValue
                        = Globals.Instance.selectedLeftClickCounty.countyData.perishableStorage / resources.Length;
                }
                else
                {
                    storageHboxes[i].maxAmountSpinBox.MaxValue
                        = Globals.Instance.selectedLeftClickCounty.countyData.nonperishableStorage / resources.Length;
                }
                storageHboxes[i].maxAmountSpinBox.Value = resources[i].maxAmount;
            }
        }

        private void UpdateMaxAmountLabels()
        {
            currentPerishableAvailableLabel.Text
                = CountStorageAmounts(Globals.Instance.selectedLeftClickCounty.countyData.perishableResources
                , Globals.Instance.selectedLeftClickCounty.countyData.perishableStorage).ToString();
            maxPerishableAmountAvailableLabel.Text
                = Globals.Instance.selectedLeftClickCounty.countyData.perishableStorage.ToString();
            currentNonperishableAvailableLabel.Text
                = CountStorageAmounts(Globals.Instance.selectedLeftClickCounty.countyData.nonperishableResources
                , Globals.Instance.selectedLeftClickCounty.countyData.nonperishableStorage).ToString();
            maxNonperisableAmountAvailableLabel.Text
                = Globals.Instance.selectedLeftClickCounty.countyData.nonperishableStorage.ToString();
        }

        private int CountStorageAmounts(ResourceData[] resources, int maxStorage)
        {
            int storage = 0;
            foreach (ResourceData resource in resources)
            {
                storage += resource.maxAmount;
            }
            int availableStorage = maxStorage - storage;
            return availableStorage;
        }

        private void CloseButtonPressed()
        {
            Hide();
        }
    }
}