namespace PlayerSpace;
public class GoodDto
{
    public string GoodName { get; set; }
    public string Description { get; set; }
    public string GoodType { get; set; }
    public string CountyGoodType { get; set; }
    public string FactionGoodType { get; set; }
    public string Perishable { get; set; }
    public int FailureRate { get; set; }
    public bool RemnantSubstitutable { get; set; }
    public bool UseRemnants { get; set; }
    public int Amount { get; set; }
    public int MaxAmount { get; set; }
    
    // Store the EquipmentType enum as a string for now
    public string EquipmentData { get; set; }
}