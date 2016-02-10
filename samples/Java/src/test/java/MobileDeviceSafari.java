import io.appium.java_client.ios.IOSDriver;
import io.appium.java_client.remote.MobileCapabilityType;
import org.apache.commons.io.FileUtils;
import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import org.openqa.selenium.OutputType;
import org.openqa.selenium.TakesScreenshot;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.openqa.selenium.remote.RemoteWebDriver;

import java.io.File;
import java.io.IOException;
import java.net.URL;


public class MobileDeviceSafari {
	
	public RemoteWebDriver driver;

	@Before
	public void setUp() throws Exception {
		DesiredCapabilities capabilities = DesiredCapabilities.iphone();
		capabilities.setCapability(MobileCapabilityType.BROWSER_NAME, "Safari");
		capabilities.setCapability(MobileCapabilityType.DEVICE_NAME, "");

		driver = new IOSDriver(
				new URL(
						"http://mobiletestingenterprise.keynote.com/appium/*****/wd/hub/"),
				capabilities);

	}

	@After
	public void tearDown() throws Exception {
		driver.close();
		driver.quit();
		Thread.sleep(10000);
	}

	@Test
	public void test1() {
			driver.get("http://www.google.com");
			screenshot("screenshot" + System.currentTimeMillis());
			driver.get("http://www.yahoo.com");
			screenshot("screenshot" + System.currentTimeMillis());
			driver.get("http://www.bing.com");
			screenshot("screenshot" + System.currentTimeMillis());
	}
	
	private void screenshot(String fileName) {
		File source = ((TakesScreenshot) driver).getScreenshotAs(OutputType.FILE);
		try {
			FileUtils.copyFile(source, new File("imgstore/" + fileName + ".png"));
			System.out.println("imgstore/" + fileName + ".png");
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

}
