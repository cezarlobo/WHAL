// Decompiled with JetBrains decompiler
// Type: Integração.WMS.Modelos.Nota_Fiscal_Item
// Assembly: Integração WMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E836F85-7F14-4ACE-B260-A3ABAFE0C808
// Assembly location: C:\Program Files (x86)\WMS\Integração\Integração WMS.exe


namespace Integração.WMS.Modelos
{
  public class Nota_Fiscal_Item
  {
    public int nf_id { get; set; }

    public int prod_id { get; set; }

    public string prod_cprod { get; set; }

    public string prod_xprod { get; set; }

    public string prod_ucom { get; set; }

    public int prod_qcom { get; set; }

    public float prod_vuncom { get; set; }

    public int prod_vprod { get; set; }

    public object prod_rastro_nlote { get; set; }

    public string data_alt { get; set; }

    public string data_add { get; set; }

    public int id { get; set; }
  }
}
