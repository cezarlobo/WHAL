// Decompiled with JetBrains decompiler
// Type: Integração.WMS.Modelos.PedidoVendaAlteracao
// Assembly: Integração WMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E836F85-7F14-4ACE-B260-A3ABAFE0C808
// Assembly location: C:\Program Files (x86)\WMS\Integração\Integração WMS.exe


namespace Integração.WMS.Modelos
{
  public class PedidoVendaAlteracao
  {
    public string token { get; set; }

    public string nota_fiscal { get; set; }

    public string serie_nota_fiscal { get; set; }

    public string tipo_frete { get; set; }

    public Notas_FiscaisPedidoVendaAlteracao[] notas_fiscais { get; set; }

    public Endereco_EntregasPedidoVendaAlteracao[] endereco_entregas { get; set; }
  }
}
