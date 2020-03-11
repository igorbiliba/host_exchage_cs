using host_exchage_cs.Host.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Host
{
    [ServiceContract]
    public interface IWCF
    {
        [OperationContract]
        [WebGet(UriTemplate = "create?amount={rubAmount}&phone={phoneFrom}&forwardtobtc={forwardtobtc}&dempingpercent={dempingpercent}&forwardint={forwardint}&forwardfraction={forwardfraction}",
                BodyStyle = WebMessageBodyStyle.Bare)]
        System.IO.Stream Create(double rubAmount, string phoneFrom, string forwardtobtc = null, int dempingpercent = 0, int forwardint = 0, string forwardfraction = "0");

        [OperationContract]
        [WebGet(UriTemplate = "check?hash={hash}",
                BodyStyle = WebMessageBodyStyle.Bare)]
        System.IO.Stream Check(string hash);

        [OperationContract]
        [WebGet(UriTemplate = "cource",
                BodyStyle = WebMessageBodyStyle.Bare)]
        System.IO.Stream Cource();

        [OperationContract]
        [WebGet(UriTemplate = "cources",
                BodyStyle = WebMessageBodyStyle.Bare)]
        System.IO.Stream Cources();

        [OperationContract]
        [WebGet(UriTemplate = "btc_validate?address={address}",
                BodyStyle = WebMessageBodyStyle.Bare)]
        System.IO.Stream BtcValidate(string address);

        [OperationContract]
        [WebGet(UriTemplate = "info",
                BodyStyle = WebMessageBodyStyle.Bare)]
        System.IO.Stream Info();
    }
}
