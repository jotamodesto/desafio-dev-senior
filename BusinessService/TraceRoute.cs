using AF = BusinessService.AddressFinder;
using RT = BusinessService.Route;
using Desafio.FCL.Enum;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessService
{
    public class TraceRoute : BaseClass
    {
        private AF.AddressFinderSoapClient afClient = new AF.AddressFinderSoapClient();
        private RT.RouteSoapClient rsClient = new RT.RouteSoapClient();

        /// <summary>
        /// Retorna o custo aproximado da rota especificada.
        /// </summary>
        /// <param name="addresses">Endereços origem e destino</param>
        /// <param name="routeType">Tipo de rota</param>
        /// <returns>Objeto com o calculo do custo da rota.</returns>
        public RouteTotals GetRouteTotals(List<Address> addresses, RouteType routeType)
        {
            var rt = new RouteTotals();
            var svcAddressesInfos = new List<AF.AddressInfo>();

            using (afClient)
            {
                var svcAddressInfo = new AF.AddressInfo();

                var svcAddressOption = new AF.AddressOptions
                {
                    searchType = 2,
                    usePhonetic = true,
                    resultRange = new AF.ResultRange { pageIndex = 1, recordsPerPage = 5 }
                };

                addresses.ForEach(address =>
                {
                    var svcAddress = new AF.Address
                    {
                        street = address.StreetName,
                        houseNumber = address.HouseNumber.ToString(),

                        city = new AF.City { name = address.City, state = address.StateProvince }
                    };

                    svcAddressInfo = afClient.findAddress(svcAddress, svcAddressOption, Token);

                    if (svcAddressInfo.addressLocation.Count > 0)
                        svcAddressesInfos.Add(svcAddressInfo);
                });
            }

            if (svcAddressesInfos.Count > 0)
            {
                using (rsClient)
                {
                    var svcRouteStops = new List<RT.RouteStop>();

                    var svcRouteOptions = new RT.RouteOptions
                    {
                        routeDetails = new RT.RouteDetails { descriptionType = 0, routeType = (int)routeType, optimizeRoute = true },

                        vehicle = new RT.Vehicle
                        {
                            tankCapacity = 30,
                            averageConsumption = 9,
                            fuelPrice = 2.8,
                            averageSpeed = 60,
                            tollFeeCat = 2
                        }
                    };

                    svcAddressesInfos.ForEach(info =>
                    {
                        var svcRS = new RT.RouteStop
                        {
                            description = info.addressLocation[0].address.street,
                            point = new RT.Point { x = info.addressLocation[0].point.x, y = info.addressLocation[0].point.y }
                        };

                        svcRouteStops.Add(svcRS);
                    });

                    var svcTotal = rsClient.getRoute(svcRouteStops, svcRouteOptions, Token);

                    rt.TotalCost = svcTotal.routeTotals.totalCost;
                    rt.TotalDistance = svcTotal.routeTotals.totalDistance;
                    rt.TotalFuelCost = svcTotal.routeTotals.totalfuelCost;
                    rt.TotalTime = System.Xml.XmlConvert.ToTimeSpan(svcTotal.routeTotals.totalTime).ToString();
                }

                return rt;
            }

            return null;
        }

        #region Dispose
        new public void Dispose()
        {
            afClient = null;
            rsClient = null;
        }
        #endregion
    }
}
