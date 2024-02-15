// Decompiled with JetBrains decompiler
// Type: Integração.WMS.Modelos.Nota_Fiscal
// Assembly: Integração WMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E836F85-7F14-4ACE-B260-A3ABAFE0C808
// Assembly location: C:\Program Files (x86)\WMS\Integração\Integração WMS.exe


namespace Integração.WMS.Modelos
{
  public class Nota_Fiscal
  {
    public int empresa { get; set; }

    public string ide_nnf { get; set; }

    public object ide_serie { get; set; }

    public string emit_cnpj { get; set; }

    public int emit_id { get; set; }

    public int dest_id { get; set; }

    public string ide_dhemi { get; set; }

    public string ide_dhsaient { get; set; }

    public string ide_tpnf { get; set; }

    public object pedido_compra { get; set; }

    public string infprot_chnfe { get; set; }

    public string nr { get; set; }

    public string data_alt { get; set; }

    public string data_add { get; set; }

    public int id { get; set; }
  }
}
