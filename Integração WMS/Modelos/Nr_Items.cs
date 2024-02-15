// Decompiled with JetBrains decompiler
// Type: Integração.WMS.Modelos.Nr_Items
// Assembly: Integração WMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E836F85-7F14-4ACE-B260-A3ABAFE0C808
// Assembly location: C:\Program Files (x86)\WMS\Integração\Integração WMS.exe


namespace Integração.WMS.Modelos
{
  public class Nr_Items
  {
    public int id { get; set; }

    public int nr_id { get; set; }

    public int empresa_id { get; set; }

    public int produto_id { get; set; }

    public object embalagem_id { get; set; }

    public string quantidade { get; set; }

    public string data_add { get; set; }

    public string data_alt { get; set; }

    public object etapa_id { get; set; }

    public string peso_liq { get; set; }

    public string peso_total { get; set; }

    public object volumes { get; set; }

    public string recebido { get; set; }

    public object user_id { get; set; }

    public object lote { get; set; }

    public bool alocacao_direta { get; set; }

    public object roteiro { get; set; }

    public object quantidade_orig { get; set; }

    public object fator_conversao_id { get; set; }

    public object unidade_medida_conversao_id { get; set; }

    public object atualizar { get; set; }

    public object etapa_id_old { get; set; }

    public object temperatura { get; set; }

    public object nf_item_id { get; set; }

    public object data_vcto { get; set; }
  }
}
