// Decompiled with JetBrains decompiler
// Type: Integração.WMS.Modelos.Nr_Item
// Assembly: Integração WMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E836F85-7F14-4ACE-B260-A3ABAFE0C808
// Assembly location: C:\Program Files (x86)\WMS\Integração\Integração WMS.exe


namespace Integração.WMS.Modelos
{
  public class Nr_Item
  {
    public int empresa_id { get; set; }

    public int nr_id { get; set; }

    public int produto_id { get; set; }

    public int quantidade { get; set; }

    public int peso_liq { get; set; }

    public int peso_total { get; set; }

    public object lote { get; set; }

    public object data_vcto { get; set; }

    public object roteiro { get; set; }

    public string data_alt { get; set; }

    public string data_add { get; set; }

    public int id { get; set; }
  }
}
