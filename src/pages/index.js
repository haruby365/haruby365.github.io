import clsx from "clsx";
import useDocusaurusContext from "@docusaurus/useDocusaurusContext";
import useBaseUrl, {useBaseUrlUtils} from "@docusaurus/useBaseUrl";
import Link from "@docusaurus/Link";

import Layout from "@theme/Layout";
import Heading from "@theme/Heading";
import IconExternalLink from "@theme/Icon/ExternalLink";

import Features from "@site/src/data/features";
import styles from "./styles.module.css";

function HeroBanner() {
    return (
        <div className={styles.hero} data-theme="dark">
            <div className={styles.heroInner}>
                <Heading as="h1">
                    <img className={styles.heroLogo}
                        alt="HARUBY"
                        src={useBaseUrl("/images/HARUBY_personalLogo.png")}
                    />
                    <span className={clsx(styles.heroTitleTextHtml, styles.heroProjectTagLine)}>
                        <b>Game</b> & <b>Character</b> <br/>
                    </span>
                    <span className={clsx(styles.heroTitleTextHtml, styles.heroProjectDescLine)}>
                        Programming, Engines, Drawing, 3D and more.
                    </span>
                </Heading>
                <div className={styles.indexCtas}>
                    <Link className={clsx("button", "button--primary", styles.indexCtasButtonExternalIcon)} href="https://www.artstation.com/haruby">
                        Artworks
                        <IconExternalLink width={"0.8em"} height={"0.8em"} />
                    </Link>
                    <Link className={clsx("button", "button--info", styles.indexCtasButtonExternalIcon)} href="https://blog.naver.com/haruby365">
                        News
                        <IconExternalLink width={"0.8em"} height={"0.8em"} />
                    </Link>
                </div>
            </div>
        </div>
    );
}

function Feature({ feature, className, }) {
    const {withBaseUrl} = useBaseUrlUtils();
    
    var imageStyle = styles.featureImageRound;
    if (feature.imageStyle === "rect") {
        imageStyle = styles.featureImageRect;
    }

    return (
        <div className={clsx("col", className, styles.featureItemContainer)}>
            <img className={imageStyle}
                alt={feature.title}
                src={withBaseUrl(feature.imageUrl)}
                loading="lazy"
            />
            <Heading as="h3" className={clsx(styles.featureHeading)}>
                {feature.title}
            </Heading>
            <p className="padding-horiz--md">{feature.description}</p>
        </div>
    );
}

function FeaturesContainer() {
    const firstRow = Features.slice(0, 2);
    const secondRow = Features.slice(2);

    return (
        <div className="container text--center">
            <div className={clsx("row", styles.featureRowFirst)}>
                {firstRow.map((feature, idx) => (
                <Feature feature={feature} key={idx} />
                ))}
            </div>
            <div className={clsx("row", styles.featureRowLast)}>
                {secondRow.map((feature, idx) => (
                    <Feature
                        feature={feature}
                        key={idx}
                        //Remove indent for 2nd row: className={clsx("col--4", idx === 0 && "col--offset-2")}
                    />
                ))}
            </div>
        </div>
    );
}

export default function Home() {
    const {siteConfig} = useDocusaurusContext();

    return (
        <Layout>
            <main>
                <HeroBanner />
                <FeaturesContainer />
            </main>
        </Layout>
    );
}
