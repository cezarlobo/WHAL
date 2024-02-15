// Decompiled with JetBrains decompiler
// Type: Integração.WMS.Modelos.Entrada
// Assembly: Integração WMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E836F85-7F14-4ACE-B260-A3ABAFE0C808
// Assembly location: C:\Program Files (x86)\WMS\Integração\Integração WMS.exe


namespace Integração.WMS.Modelos
{
  public class Entrada
  {
    public string token { get; set; }

    public string nota_fiscal { get; set; }

    public string data_emissao { get; set; }

    public string emitente { get; set; }

    public string chave_acesso { get; set; }

    public string nr { get; set; }

    public string tipo { get; set; }

    public string apontamento_producao { get; set; }

    public ItenEntrada[] itens { get; set; }
  }
}
