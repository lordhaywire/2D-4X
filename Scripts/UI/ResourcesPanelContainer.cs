using Godot;
using System;
using System.Diagnostics.Metrics;

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

        private void AssignResourcesToStorageHboxes()
        {
            County county = Globals.Instance.selectedLeftClickCounty;
            GD.Print("County: " + county.countyData.countyName);
            for (int i = 0; i < perishableResourceStorageHbox.Length; i++)
            {
                GD.Print("County Resource: " + county.countyData.perishableResources[i]);
                perishableResourceStorageHbox[i].resourceData = county.countyData.perishableResources[i];
            }
            for (int i = 0; i < nonperishableResourceStorageHbox.Length; i++)
            {
                nonperishableResourceStorageHbox[i].resourceData = county.countyData.nonperishableResources[i];
            }
        }

        private void OnVisibilityChanged()
        {
            if (Visible)
            {
                AssignResourcesToStorageHboxes();
                PlayerControls.Instance.playerControlsEnabled = false;
                UpdateMaxAmountLabels();
                UpdateResourceLabels();
            }
            else
            {
                // Set the county resource max storage to equal whatever the player left it as.
                SetResourceMaxValues(perishableResourceStorageHbox);
                SetResourceMaxValues(nonperishableResourceStorageHbox);

                PlayerControls.Instance.playerControlsEnabled = true;
            }
        }

        
        private void SetResourceMaxValues(StorageHbox[] storageHbox)
        {
            for(int i = 0; i < storageHbox.Length; i++)
            {
                storageHbox[i].resourceData.MaxAmount = (int)storageHbox[i].maxAmountSpinBox.Value;
            }
        }

        public void UpdateSpinBoxMaxValue(bool perishable)
        {
            if (perishable)
            {
                UpdateMaxValue(perishableResourceStorageHbox, currentPerishableAvailableLabel);
            }
            else
            {
                UpdateMaxValue(nonperishableResourceStorageHbox, currentNonperishableAvailableLabel);
            }
        }

        private void UpdateMaxValue(StorageHbox[] storageHboxArray, Label availableLabel)
        {
            foreach (StorageHbox storageHbox in storageHboxArray)
            {
                storageHbox.maxAmountSpinBox.MaxValue = storageHbox.maxAmountSpinBox.Value + int.Parse(availableLabel.Text);
            }
        }

        public void UpdateCountyAvailableStorage(bool perishable)
        {
            CountyData countyData = Globals.Instance.selectedLeftClickCounty.countyData;
            if (perishable)
            {
                int totalUsedStorage = 0;
                foreach (StorageHbox storageHBox in perishableResourceStorageHbox)
                {
                    totalUsedStorage += (int)storageHBox.maxAmountSpinBox.Value;
                }
                int totalAvailableStorage = countyData.perishableStorage - totalUsedStorage;
                currentPerishableAvailableLabel.Text = totalAvailableStorage.ToString();
            }
            else
            {
                int totalUsedStorage = 0;
                foreach (StorageHbox storageHBox in nonperishableResourceStorageHbox)
                {
                    totalUsedStorage += (int)storageHBox.maxAmountSpinBox.Value;
                }
                int totalAvailableStorage = countyData.nonperishableStorage - totalUsedStorage;
                currentNonperishableAvailableLabel.Text = totalAvailableStorage.ToString();
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
                GD.Print(resources[i].MaxAmount);
                storageHboxes[i].resourceMaxAmountLabel.Text
                    = resources[i].MaxAmount.ToString();

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
                storageHboxes[i].maxAmountSpinBox.Value = resources[i].MaxAmount;
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
                storage += resource.MaxAmount;
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