namespace Chromia.PostchainClient.GTX
{
    public class GTXClient
    {
        private RESTClient RestApiClient;
        private string BlockchainRID;

        public GTXClient(RESTClient restApiClient, string blockchainRID)
        {
            this.RestApiClient = restApiClient;
            this.BlockchainRID = blockchainRID;
        }

        public Transaction NewTransaction(byte[][] signers)
        {
            Gtx newGtx = new Gtx(this.BlockchainRID);

            foreach(byte[] signer in signers)
            {
                newGtx.AddSignerToGtx(signer);
            }

            Transaction req = new Transaction(newGtx, this.RestApiClient);
            
            return req;
        }

        public Transaction TransactionFromRawTransaction(byte[] rawTransaction)
        {
            Gtx gtx = Gtx.Deserialize(rawTransaction);

            Transaction req = new Transaction(gtx, this.RestApiClient);

            return req;
        }        
    }
}