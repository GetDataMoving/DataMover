# import os
# import sys
# import DataMover

# #StoredKeyFunctions.SetKeyStoreValue("Ig", "test")
# integrationsConnectionString = StoredKeyFunctions.GetKeyStoreValue("IntegrationsSQLConnectionString")
# thumbItConnectionString = StoredKeyFunctions.GetKeyStoreValue("ThumbItOTAIntegTest")
# delimitedFilePath = os.path.join(currentDirectoryPath, "test.psv")
# # result = DataMover.GetVersion()
# # print(result.Command)
# # print(result.StandardOutput)
# result2 = DataMover.PG2DF(thumbItConnectionString, "public", "count", delimitedFilePath, "|", True, None, "Name", True, "Verbose")
# print(result2.StandardOutput)
# print(result2.StandardError)
# print(result2.Command)
