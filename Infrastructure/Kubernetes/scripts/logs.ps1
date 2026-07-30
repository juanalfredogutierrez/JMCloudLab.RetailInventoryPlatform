param(
    [Parameter(Mandatory = $true)]
    [string]$Pod
)

kubectl logs -f $Pod -n retailinventory