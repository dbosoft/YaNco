namespace ExportMATMAS;

public record IDocDataRecord(string IDocNo, string Segment, int SegmentNo, int ParentNo, int Level, string Data);