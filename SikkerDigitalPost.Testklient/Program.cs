﻿using System;
using System.Security.Cryptography.X509Certificates;
using SikkerDigitalPost.Domene.Entiteter;
using SikkerDigitalPost.Domene.Entiteter.Aktører;
using SikkerDigitalPost.Domene.Entiteter.Kvitteringer;
using SikkerDigitalPost.Domene.Entiteter.Post;
using SikkerDigitalPost.Domene.Enums;
using SikkerDigitalPost.Klient;
using SikkerDigitalPost.Klient.Utilities;

namespace SikkerDigitalPost.Testklient
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * Følgende sertifikater må brukes for å kunne sende digital post
             * 
             * - Mottagersertifikat brukes for å kryptere og signere dokumentpakke som skal til mottagerens postkasse.
             * - TekniskAvsenderSertifikat brukes for sikker kommunikasjon med meldingsformidler.
             */

            X509Certificate2 tekniskAvsenderSertifikat;
            { 
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                tekniskAvsenderSertifikat = store.Certificates.Find(
                    X509FindType.FindByThumbprint, "8702F5E55217EC88CF2CCBADAC290BB4312594AC", true)[0];
                store.Close();
            }

            X509Certificate2 mottakerSertifikat;
            {
                X509Store store2 = new X509Store(StoreName.TrustedPeople, StoreLocation.CurrentUser);
                store2.Open(OpenFlags.ReadOnly);
                mottakerSertifikat =
                    store2.Certificates.Find(X509FindType.FindByThumbprint, "B43CAAA0FBEE6C8DA85B47D1E5B7BCAB42AB9ADD", true)[0];
                store2.Close();
            }

            /*
             * I dette eksemplet er det Posten som er den som produserer informasjon/brev/post som skal formidles (Behandlingsansvarlig),
             * Posten som er teknisk avsender, og det er Digipostkassen som skal motta meldingen. Derfor er alle organisasjonsnummer
             * identiske. 
             */

            var organisasjonsnummerEboks = "996460320";
            var fødselsnummerEboks = "01043100358";
            var postkasseadresseEboks = "";

            var organisasjonsnummerPosten = "984661185";
            var organisasjonsnummerTekniskAvsender = organisasjonsnummerPosten;
            var organisasjonsnummerMottagerPostkasse = organisasjonsnummerPosten;
            var organisasjonsnummerBehandlingsansvarlig = organisasjonsnummerPosten;

            //Avsender
            var behandlingsansvarlig = new Behandlingsansvarlig(new Organisasjonsnummer(organisasjonsnummerBehandlingsansvarlig));
            var tekniskAvsender = new Databehandler(organisasjonsnummerTekniskAvsender, tekniskAvsenderSertifikat);

            //Mottaker
            var mottaker = new Mottaker("04036125433", "ove.jonsen#6K5A", mottakerSertifikat, organisasjonsnummerMottagerPostkasse);

            //Digital Post
            var digitalPost = new DigitalPost(mottaker, "Ikke-sensitiv tittel");

            string hoveddokument = FileUtility.AbsolutePath("testdata", "hoveddokument", "hoveddokument.txt");
            string vedlegg = FileUtility.AbsolutePath("testdata", "vedlegg", "Vedlgg.txt");
            
            //Forsendelse
            var dokumentpakke = new Dokumentpakke(new Dokument("Hoveddokument", hoveddokument, "text/plain"));
            dokumentpakke.LeggTilVedlegg(new Dokument("Vedlegg",vedlegg,"text/plain","EN"));
            var forsendelse = new Forsendelse(behandlingsansvarlig, digitalPost, dokumentpakke, Prioritet.Prioritert);

            //Send
            var sikkerDigitalPostKlient = new SikkerDigitalPostKlient(tekniskAvsender);
            Transportkvittering transportkvittering = sikkerDigitalPostKlient.Send(forsendelse);

            //Hent kvittering
            var kvitteringsForespørsel = new Kvitteringsforespørsel(Prioritet.Prioritert);
            Forretningskvittering kvittering = sikkerDigitalPostKlient.HentKvittering(kvitteringsForespørsel);

            //Bekreft mottak av kvittering. 
            sikkerDigitalPostKlient.Bekreft(kvittering);
        }
    }
}





