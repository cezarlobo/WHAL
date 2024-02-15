// Decompiled with JetBrains decompiler
// Type: Integração.WMS.Modelos.Pedido
// Assembly: Integração WMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E836F85-7F14-4ACE-B260-A3ABAFE0C808
// Assembly location: C:\Program Files (x86)\WMS\Integração\Integração WMS.exe


namespace Integração.WMS.Modelos
{
  public class Pedido
  {
    public string observacao { get; set; }

    public string cliente_cnpj { get; set; }

    public string pedido_venda { get; set; }

    public string ordem_montagem { get; set; }

    public string data_entrega { get; set; }

    public string cod_rota { get; set; }

    public string sequencia_entrega { get; set; }

    public string transp_nome { get; set; }

    public string qtde_itens { get; set; }

    public string prefixo { get; set; }

    public string cod_ponto_entrega { get; set; }

    public string desc_ponto_entrega { get; set; }

    public string canal { get; set; }

    public Iten[] itens { get; set; }

    public Notas_FiscaisPedidoVenda[] notas_fiscais { get; set; }

    public Endereco_EntregasPedidoVenda[] endereco_entregas { get; set; }
  }
}
