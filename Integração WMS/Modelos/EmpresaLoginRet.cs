// Decompiled with JetBrains decompiler
// Type: Integração.WMS.Modelos.EmpresaLoginRet
// Assembly: Integração WMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E836F85-7F14-4ACE-B260-A3ABAFE0C808
// Assembly location: C:\Program Files (x86)\WMS\Integração\Integração WMS.exe


namespace Integração.WMS.Modelos
{
  public class EmpresaLoginRet
  {
    public int id { get; set; }

    public string xnome { get; set; }

    public object xfant { get; set; }

    public string cnpj_cpf { get; set; }

    public object id_estrangeiro { get; set; }

    public object ie { get; set; }

    public object ind_ie { get; set; }

    public object im { get; set; }

    public object isuf { get; set; }

    public object cnae { get; set; }

    public object crt { get; set; }

    public object email { get; set; }

    public object fone { get; set; }

    public object propriedade { get; set; }

    public string ativo { get; set; }

    public int empresa { get; set; }

    public bool principal { get; set; }

    public string data_add { get; set; }

    public string data_alt { get; set; }

    public bool remove_zero_esquerda { get; set; }

    public object num_decimal_nota { get; set; }

    public object capacidade_recebimento_dia { get; set; }

    public object capacidade_expedicao_dia { get; set; }

    public object ocupacao_maxima { get; set; }

    public object aging_maximo { get; set; }

    public int nfe_cli { get; set; }

    public string agrupamento_nr { get; set; }

    public int volume_id { get; set; }

    public bool double_check_fiscal { get; set; }

    public bool ignora_destinatario_xml_entrada { get; set; }

    public bool gerar_pedido_pre { get; set; }

    public bool is_filial { get; set; }

    public bool conferencia_pedido_vcto { get; set; }

    public bool vincular_cliente_hu { get; set; }

    public bool recebimento_romaneio { get; set; }

    public bool imprimir_expedicao_notafiscal { get; set; }

    public int termo_id { get; set; }
  }
}
