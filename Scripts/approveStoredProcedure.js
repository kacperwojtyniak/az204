function approveOrder(id) {
    var collection = getContext().getCollection();
    var collectionLink = collection.getSelfLink();
    var response = getContext().getResponse();

    update();

    function update() {
        var query = { query: "select * from c where c.id = @id", parameters: [{ name: "@id", value: id }] };

        var isAccepted = collection.queryDocuments(collectionLink, query, function (err, documents, responseOptions) {
            if (err) throw err;

            if (documents.length > 0) {
                var order = documents[0];
                order.approved = true;
                collection.replaceDocument(order._self, order, function (err, updatedDocument, responseOptions) {
                    response.setBody(updatedDocument);
                });
            } else {
                throw new Error("Document not found.");
            }
        });
        if (!isAccepted) {
            throw new Error("The stored procedure timed out.");
        }
    }
}
