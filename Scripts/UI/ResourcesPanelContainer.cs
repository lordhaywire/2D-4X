using Godot;
using System;
using System.Collections.Generic;
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
            int perishable = 0;
            int nonperishable = 0;
            foreach (KeyValuePair<AllEnums.CountyResourceType, ResourceData> keyValuePair 
                in county.countyData.perishableResources)
            {
                perishableResourceStorageHbox[perishable].resourceData = keyValuePair.Value;
                perishable++;
            }

            foreach (KeyValuePair<AllEnums.CountyResourceType, ResourceData> keyValuePair 
                in county.countyData.nonperishableResources)
            {
                nonperishableResourceStorageHbox[nonperishable].resourceData = keyValuePair.Value;
                nonperishable++;
            }
        }

        private void OnVisibilityChanged()
        {
            if (Visible)
            {
                AssignResourcesToStorageHboxes();
                UpdateMaxAmountLabels();
                UpdateResourceLabels();
                Clock.Instance.PauseandUnpause();
                PlayerControls.Instance.playerControlsEnabled = false;
            }
            else
            {
                // Set the county resource max storage to equal whatever the player left it as.
                SetResourceMaxValues(perishableResourceStorageHbox);
                SetResourceMaxValues(nonperishableResourceStorageHbox);

                Clock.Instance.PauseandUnpause();
                PlayerControls.Instance.playerControlsEnabled = true;
            }
        }

        private static void SetResourceMaxValues(StorageHbox[] storageHbox)
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

        private static void UpdateMaxValue(StorageHbox[] storageHboxArray, Label availableLabel)
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

        private static void UpdateEachTypeOfResource(StorageHbox[] storageHboxes
            , Godot.Collections.Dictionary<AllEnums.CountyResourceType, ResourceData> resources)
        {
            int i = 0;
            foreach (KeyValuePair<AllEnums.CountyResourceType, ResourceData> keyValuePair in resources)
            {
                //AllEnums.CountyResourceType key = keyValuePair.Key;
                ResourceData resource = keyValuePair.Value;

                storageHboxes[i].perishable = resource.perishable;
                storageHboxes[i].storageHboxIndex = i;
                storageHboxes[i].resourceNameLabel.Text = $"{resource.resourceName}:";
                storageHboxes[i].resourceAmountLabel.Text = resource.amount.ToString();
                GD.Print(resource.MaxAmount);
                storageHboxes[i].resourceMaxAmountLabel.Text = resource.MaxAmount.ToString();

                // Spinbox
                if (resource.perishable)
                {
                    storageHboxes[i].maxAmountSpinBox.MaxValue 
                        = Globals.Instance.selectedLeftClickCounty.countyData.perishableStorage / resources.Count;
                }
                else
                {
                    storageHboxes[i].maxAmountSpinBox.MaxValue 
                        = Globals.Instance.selectedLeftClickCounty.countyData.nonperishableStorage / resources.Count;
                }
                storageHboxes[i].maxAmountSpinBox.Value = resource.MaxAmount;
                i++;
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

        private static int CountStorageAmounts(Godot.Collections.Dictionary<AllEnums.CountyResourceType, ResourceData> resources
            , int maxStorage)
        {
            int storage = 0;
            foreach (ResourceData resource in resources.Values)
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