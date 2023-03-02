namespace ExportMATMAS;

public record MaterialMasterRecord(string MaterialNo, ClientData ClientData, DescriptionData[] Descriptions, PlantData[] PlantData);