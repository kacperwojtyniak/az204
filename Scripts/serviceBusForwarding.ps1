Connect-AzAccount

$queue1Name = 'firstqueue'
$queue2Name = 'secondqueue'

$resourceGroupName = 'TestTemplates'
$namespace = 'az204allinone'


# Create receiving queue
New-AzServiceBusQueue `
-ResourceGroup $resourceGroupName `
-NamespaceName $namespace `
-QueueName $queue2Name `
-EnablePartitioning $False

#Create forwarding queue
New-AzServiceBusQueue `
-ResourceGroup $resourceGroupName `
-NamespaceName $namespace `
-QueueName $queue1Name `
-EnablePartitioning $False `
-ForwardTo $queue2Name


#Topic stuff
$queue3Name = 'topicqueue'
$topicName = 'firsttopic'
$queue4Name = 'queue1'
$subscription1Name = 'queue1sub'
$queue5Name = 'queue2'
$subscription2Name = 'queue2sub'

#
#                     ----->queue4
#                    |
# queue3 --> topic1 -
#                    |
#                     ----->queue5
#

#create topic
New-AzServiceBusTopic `
-ResourceGroupName $resourceGroupName `
-Namespace $namespace `
-Name $topicName `
-EnablePartitioning $True

#Create first receiving queue
New-AzServiceBusQueue `
-ResourceGroup $resourceGroupName `
-NamespaceName $namespace `
-QueueName $queue4Name `
-EnablePartitioning $False

#Create second receiving queue
New-AzServiceBusQueue `
-ResourceGroup $resourceGroupName `
-NamespaceName $namespace `
-QueueName $queue5Name `
-EnablePartitioning $False

#Create queue that forwards messages to topic
New-AzServiceBusQueue `
-ResourceGroup $resourceGroupName `
-NamespaceName $namespace `
-QueueName $queue3Name `
-EnablePartitioning $False `
-ForwardTo $topicName

#Create subscription to forward to queue4
New-AzServiceBusSubscription `
-ResourceGroupName $resourceGroupName `
-Namespace $namespace `
-Topic $topicName `
-Name $subscription1Name `
-ForwardTo $queue4Name

#Create subscription to forward to queue5
New-AzServiceBusSubscription `
-ResourceGroupName $resourceGroupName `
-Namespace $namespace `
-Topic $topicName `
-Name $subscription2Name `
-ForwardTo $queue5Name