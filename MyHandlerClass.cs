using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using System.Threading.Tasks;

namespace HelloLambdaNamespace
{
  public class MyHandlerClass {
   [LambdaSerializer(typeof(JsonSerializer))]
   public async Task<Result> HandleFunction(Request request)
   {
    return new Result 
    {
      HelloWorld=request.Name
    };
   }
 }
 public class Request{
  public string Name { get; set; }
 }
 public class Result {
  public string HelloWorld { get; set; }
 }
}