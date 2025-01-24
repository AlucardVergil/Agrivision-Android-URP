using System.Collections.Generic;
using System.Numerics;

[System.Serializable]
public class ParcelsData
{
    public Parcel[] parcels;
}


[System.Serializable]
public class Parcel
{
    public int id;
    public string name;
    public float size;
    public string crop_type;
    public string sowing_date;
    public string harvesting_date;
    public Shape shape;
    public Bbox bbox;
    public bool crop_type_support_diseases;
}


[System.Serializable]
public class Shape
{
    public string type;
    public float[][][] coordinates;
}


[System.Serializable]
public class Bbox
{
    public string type;
    public float[][][] coordinates;
}