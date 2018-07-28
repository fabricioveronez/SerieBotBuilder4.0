﻿using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts;
using System;
using System.Linq;
using Microsoft.Recognizers.Text;
using System.Collections.Generic;
using static Microsoft.Bot.Builder.Prompts.DateTimeResult;

namespace ExemploDialogo
{
    public class Jarvis : IBot
    {

        DialogSet dialogos;
        string nomeCliente;
        string convenio;
        DateTime dataHora;

        string saborPizza;
        string tamanhoPizza;
        string enderecoEntrega;
        string formaPagamento;
        string valorTroco;

        public Jarvis()
        {
            dialogos = new DialogSet();


            // Dialogo pra marcar consulta
            dialogos.Add("obterNome", new WaterfallStep[] {
                async (dc, args, next) =>
                {
                    await dc.Prompt("capturaTexto","Qual o seu nome ?");
                },
                async (dc, args, next) =>
                {
                    nomeCliente = ((TextResult)args).Value;
                    await dc.End();
                }
            });

            // Dialogo pra marcar consulta
            dialogos.Add("marcarConsulta", new WaterfallStep[] {
                async (dc, args, next) =>
                {
                    await dc.Begin("obterNome");
                },
                async (dc, args, next) =>
                {
                    nomeCliente = ((TextResult)args).Value;
                    await dc.Prompt("capturaTexto",$"{nomeCliente}, qual o convênio ?");
                },
                async (dc, args, next) =>
                {
                    convenio = ((TextResult)args).Value;
                    await dc.Prompt("capturaDataHora","Certo... Qual o dia e horário ?");
                },
                async (dc, args, next) =>
                {
                    DateTimeResolution resultado = ((DateTimeResult)args).Resolution.First();
                    dataHora = Convert.ToDateTime(resultado.Value);

                    await dc.Context.SendActivity($"Está marcado. Dia {dataHora.ToString("dd/MM/yyyy HH:mm:ss")}");
                    await dc.End();
                },
            });

            // Pedir pizza 
            dialogos.Add("pedirPizza", new WaterfallStep[] {
                async (dc, args, next) =>
                {
                    await dc.Prompt("capturaTexto",$"Qual o sabor da pizza {nomeCliente} ?");
                },
                async (dc, args, next) =>
                {
                    saborPizza = ((TextResult)args).Value;
                    await dc.Prompt("capturaTexto","Qual o tamanho ?");
                },
                async (dc, args, next) =>
                {
                    tamanhoPizza = ((TextResult)args).Value;
                    await dc.Prompt("capturaTexto","Qual o endereço de entrega ?");
                },
                async (dc, args, next) =>
                {
                    enderecoEntrega = ((TextResult)args).Value;
                    await dc.Prompt("capturaTexto","Qual a forma de pagamento ?");
                },
                async (dc, args, next) =>
                {
                    formaPagamento = ((TextResult)args).Value;
                    await dc.Prompt("capturaTexto","Ok, troco pra quanto ?");
                },
                async (dc, args, next) =>
                {
                    valorTroco = ((TextResult)args).Value;
                    await dc.Prompt("capturaTexto",$"Ok {nomeCliente}, o pedido será entregue por volta de 40 minutos.");
                    await dc.End();
                }
            });

            dialogos.Add("capturaDataHora", new Microsoft.Bot.Builder.Dialogs.DateTimePrompt(Culture.Portuguese));
            dialogos.Add("capturaTexto", new Microsoft.Bot.Builder.Dialogs.TextPrompt());
        }

        public async Task OnTurn(ITurnContext context)
        {

            Dictionary<string, object> state = ConversationState<Dictionary<string, object>>.Get(context);
            DialogContext dc = dialogos.CreateContext(context, state);
            await dc.Continue();

            if (context.Activity.Type == ActivityTypes.Message)
            {
                if (!context.Responded)
                {
                    await dc.Begin("marcarConsulta");
                }
            }
        }
    }
}
