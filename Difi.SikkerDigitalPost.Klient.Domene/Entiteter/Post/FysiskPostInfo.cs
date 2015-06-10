﻿using Difi.SikkerDigitalPost.Klient.Domene.Entiteter.Aktører;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter.FysiskPost;
using Difi.SikkerDigitalPost.Klient.Domene.Enums;

namespace Difi.SikkerDigitalPost.Klient.Domene.Entiteter.Post
{
    /// <summary>
    /// Inneholder nødvendig informasjon for å sende et brev digitalt til Digipost eller Eboks, slik 
    /// som mottaker, posttype, om brevet skal skrives ut med farge, hvordan det skal håndteres hvis
    /// det ikke blir levert og en returmottaker.
    /// </summary>
    public class FysiskPostInfo : PostInfo
    {
        public Posttype Posttype { get; set; }

        public Utskriftsfarge Utskriftsfarge { get; set; }

        public Posthåndtering Posthåndtering { get; set; }

        public FysiskPostMottaker ReturMottaker { get; set; }

        public FysiskPostInfo(PostMottaker mottaker, Posttype posttype, Utskriftsfarge utskriftsfarge, Posthåndtering posthåndtering, FysiskPostMottaker returMottaker) : base(mottaker)
        {
            Posttype = posttype;
            Utskriftsfarge = utskriftsfarge;
            Posthåndtering = posthåndtering;
            ReturMottaker = returMottaker;
        }
    }
}