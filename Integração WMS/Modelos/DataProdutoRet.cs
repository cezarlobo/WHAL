// Decompiled with JetBrains decompiler
// Type: Integração.WMS.Modelos.DataProdutoRet
// Assembly: Integração WMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E836F85-7F14-4ACE-B260-A3ABAFE0C808
// Assembly location: C:\Program Files (x86)\WMS\Integração\Integração WMS.exe

namespace Integração.WMS.Modelos
{
  public class DataProdutoRet
  {
    public string codigo { get; set; }

    public string pn { get; set; }

    public string descricao { get; set; }

    public string ncm { get; set; }

    public string unid_medida { get; set; }

    public int grupo { get; set; }

    public string tipo { get; set; }

    public string altura { get; set; }

    public string largura { get; set; }

    public string comprimento { get; set; }

    public int peso_unit { get; set; }

    public int min_dias_estoque { get; set; }

    public int max_dias_estoque { get; set; }

    public bool mostra_analise_consumo { get; set; }

    public bool lote { get; set; }

    public bool serie { get; set; }

    public bool validade { get; set; }

    public bool inspecao_rec { get; set; }

    public bool inspecao_exp { get; set; }

    public bool laudo_rec { get; set; }

    public bool laudo_exp { get; set; }

    public string tipo_picking { get; set; }

    public int estoque_min { get; set; }

    public int estoque_max { get; set; }

    public int ponto_ressuprimento { get; set; }

    public int multiplo_ressuprimento { get; set; }

    public object origem_ressuprimento { get; set; }

    public object fornecedor { get; set; }

    public string categoria { get; set; }

    public int empresa { get; set; }

    public string data_alt { get; set; }

    public string data_add { get; set; }

    public int id { get; set; }
  }
}
