// Decompiled with JetBrains decompiler
// Type: Integração.WMS.Modelos.Nr
// Assembly: Integração WMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E836F85-7F14-4ACE-B260-A3ABAFE0C808
// Assembly location: C:\Program Files (x86)\WMS\Integração\Integração WMS.exe


namespace Integração.WMS.Modelos
{
  public class Nr
  {
    public int empresa_id { get; set; }

    public int nr_status_id { get; set; }

    public int nf_id { get; set; }

    public int fornecedor_id { get; set; }

    public string numero { get; set; }

    public object serie { get; set; }

    public object pedido { get; set; }

    public string tipo { get; set; }

    public object observacao { get; set; }

    public object devolucao { get; set; }

    public object numero_carga { get; set; }

    public string data_alt { get; set; }

    public string data_add { get; set; }

    public int id { get; set; }

    public Nr_Items[] nr_items { get; set; }
  }
}
